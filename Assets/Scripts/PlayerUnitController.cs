using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Custom.Interfaces;
using Custom.Extensions;

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
    //both lists because stuff can die
    public List<FlockAgent> selected = new List<FlockAgent>();
    public List<FlockAgent> finalSelected = new List<FlockAgent>();
    private bool isSelectingPosition;
    private bool selectedChecked = false;
    private int lastCheckedCount = 0;
    private bool specificSelection;
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
            UpdateSelectionBox();
            selected.SetColour(Color.white);
            selected.Clear();
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.MousePositionWorld(mousePositionScreen), (LayerMask)(1 << 7));
            GetAgents(selectedColliders2D, ref selected);
        }
        moveSetCooldown -= Time.deltaTime;
        if (isSelectingPosition)
        {
            MoveUnit();
        }
    }
    private void LateUpdate()
    {
        if (specificSelection)
        {
            finalSelected.Where(x => !selected.Contains(x)).SetColour(Color.cyan);
            selected.Where(x => !finalSelected.Contains(x)).SetColour(Color.cyan);
            return;
        }
        selected.SetColour(Color.yellow);
        finalSelected.SetColour(Color.cyan);
    }
    private void OnSelect(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            selectionBox.enabled = true;
            selectionStartScreen = mousePositionScreen;
            Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (LayerMask)((1 << 6)));
            selected.Clear();
            GetAgents(selectedColliders2D, ref selected);
            return;
        }
        selectionStartScreen = null;
        selectionBox.enabled = false;
        finalSelected.SetColour(Color.white);
        if (specificSelection)
        {
            finalSelected = finalSelected
                .Where(x => !selected.Contains(x))
                .Concat(selected.Where(x => !finalSelected.Contains(x)))
                .ToList();
        }
        else
        {
            finalSelected.Clear();
            finalSelected = new List<FlockAgent>(selected);
        }
        selectedChecked = false;
        selected.Clear();
    }
    private void OnMoveUnit(InputValue inputValue)
    {
        isSelectingPosition = inputValue.Get<float>() > 0;
    }
    private void GetAgents(Collider2D[] collider2Ds, ref List<FlockAgent> flockAgents)
    {
        foreach (Collider2D collider in collider2Ds)
        {
            if (!FlockAgent.ships.TryGetValue(collider, out FlockAgent ship))
            {
                continue;
            }
            if (!flockAgents.Contains(ship))
            {
                flockAgents.Add(ship);
            }
        }
    }
    private void OnMousePosition(InputValue inputValue)
    {
        mousePositionScreen = inputValue.Get<Vector2>();
    }
    private void MoveUnit()
    {
        Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld((Vector2)mousePositionScreen), (LayerMask)~(1 << 6));
        FlockAgent selectedTarget = null;
        for (int i = 0; i < selectedColliders2D.Length; i++)
        {
            if (!FlockAgent.ships.ContainsKey(selectedColliders2D[i]))
            {
                continue;
            }
            if (FlockAgent.ships[selectedColliders2D[i]].Team == 1)
            {
                continue;
            }
            selectedTarget = FlockAgent.ships[selectedColliders2D[i]];
            break;
        }
        if (moveSetCooldown >= 0 && selectedTarget == null)
        {
            return;
        }
        if (finalSelected.Count <= 0)
        {
            return;
        }
        if (selectionStartScreen != null)
        {
            return;
        }
        if (!selectedChecked || lastCheckedCount != finalSelected.Count)
        {
            finalSelected = finalSelected.OrderBy(agent => agent.gameObject.GetInstanceID()).ToList();
            selectedChecked = true;
            lastCheckedCount = finalSelected.Count;
        }
        for (int i = 0; i < finalSelected.Count; i++)
        {
            finalSelected[i].dogFighting = selectedTarget != null;
        }
        Flock.SetDestination(CameraControl.Singleton.MousePositionWorld(mousePositionScreen), finalSelected);
        moveSetCooldown = 1f;
    }
    private void UpdateSelectionBox()
    {
        RectTransform selectionBoxTransform = selectionBox.rectTransform;
        selectionBoxTransform.anchoredPosition = ((Vector3)selectionStartScreen + mousePositionScreen) * 0.5f / GameManager.Singleton.canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = ((Vector2)mousePositionScreen - (Vector2)selectionStartScreen) / GameManager.Singleton.canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = new Vector2(Mathf.Abs(selectionBoxTransform.sizeDelta.x), Mathf.Abs(selectionBoxTransform.sizeDelta.y));
    }
    private void OnDeselect(InputValue inputValue)
    {
        if (inputValue.Get<float>() == 0) { return; }
        foreach (FlockAgent item in finalSelected) { ((IShip)item).SetColour(Color.white); }
        finalSelected.Clear();
        selectionStartScreen = null;
        selectionBox.enabled = false;
    }
    private void OnSpecificSelect(InputValue inputValue)
    {
        specificSelection = inputValue.Get<float>() > 0;
    }
}
