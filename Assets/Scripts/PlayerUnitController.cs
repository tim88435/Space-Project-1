using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Custom.Interfaces;
using Custom.Extensions;
using Custom.StaticClasses;

public class PlayerUnitController : MonoBehaviour
{
    private static PlayerUnitController _singleton;
    public static PlayerUnitController Singleton
    {
        get { return _singleton; }
        set
        {
            if (_singleton == null)
            {
                _singleton = value;
                return;
            }
            if (_singleton != value)
            {
                Debug.LogWarning($"Component {nameof(Singleton)} already exists in current scene\nRemoving duplicate");
            }
        }
    }
    [SerializeField] private Image selectionBox;
    public List<FlockAgent> selected = new List<FlockAgent>();
    private bool isSelectingPosition;
    private bool selectedChecked = false;
    private float moveSetCooldown = 1f;
    private Vector2? selectionStartScreen;
    private Vector3 mousePositionScreen = Vector2.zero;
    private void OnEnable()
    {
        Singleton = this;
    }
    private void Update()
    {
        if (selectionStartScreen != null)
        {
            selectedChecked = false;
            UpdateSelectionBox();
            foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
            selected.Clear();
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.MousePositionWorld(mousePositionScreen), (LayerMask)(1 << 7));
            SelectColliders(selectedColliders2D);
        }
        moveSetCooldown -= Time.deltaTime;
        if (isSelectingPosition)
        {
            MoveUnit();
        }
    }
    private void OnSelect(InputValue inputValue)
    {
        foreach (FlockAgent item in selected)
        {
            if (item == null)
            {
                selected.Remove(item);
                continue;
            }
            ((ISelectable)item).SetColour(Color.white);
        }
        if (inputValue.Get<float>() > 0)
        {
            selected.Clear();
            selectionBox.enabled = true;
            selectionStartScreen = mousePositionScreen;
            Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (LayerMask)((1 << 6)));
            SelectColliders(selectedColliders2D);
            return;
        }
        selectionBox.enabled = false;
        for (int i = 0; i < selected.Count; i++)
        {
            ((ISelectable)selected[i]).SetColour(Color.cyan);
        }
        selectionStartScreen = null;
    }
    private void OnMoveUnit(InputValue inputValue)
    {
        if (!isSelectingPosition)
        {
            bool dogFighting = false;
            Collider2D collider2D = CameraControl.CameraCast(mousePositionScreen);
            if (collider2D != null)
            {
                if (FlockAgent.ships.TryGetValue(collider2D, out FlockAgent flockAgent))
                {
                    if (flockAgent.Team != 1)
                    {
                        dogFighting = true;
                    }
                }
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].dogFighting = dogFighting;
                }
            }
        }
        isSelectingPosition = inputValue.Get<float>() > 0;
    }
    private void SelectColliders(Collider2D[] collider2Ds)
    {
        foreach (Collider2D collider in collider2Ds)
        {
            if (!FlockAgent.ships.TryGetValue(collider, out FlockAgent ship))
            {
                return;
            }
            selected.Add(ship);
            ((ISelectable)ship).SetColour(Color.yellow);
        }
    }
    private void OnMousePosition(InputValue inputValue)
    {
        mousePositionScreen = inputValue.Get<Vector2>();
    }
    private void MoveUnit()
    {
        if (moveSetCooldown >= 0)
        {
            return;
        }
        if (selected.Count <= 0)
        {
            return;
        }
        if (selectionStartScreen != null)
        {
            return;
        }
        if (!selectedChecked)
        {
            selected = selected.OrderBy(agent => agent.gameObject.GetInstanceID()).ToList();
            selectedChecked = true;
        }
        Flock.SetDestination(CameraControl.Singleton.MousePositionWorld(mousePositionScreen), selected);
        moveSetCooldown = 0.1f;
    }
    private void UpdateSelectionBox()
    {
        RectTransform selectionBoxTransform = selectionBox.rectTransform;
        selectionBoxTransform.anchoredPosition = ((Vector3)selectionStartScreen + mousePositionScreen) * 0.5f / GameManager.Singleton.canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = ((Vector2)mousePositionScreen - (Vector2)selectionStartScreen) / GameManager.Singleton.canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = new Vector2(Mathf.Abs(selectionBoxTransform.sizeDelta.x), Mathf.Abs(selectionBoxTransform.sizeDelta.y));
    }
}
