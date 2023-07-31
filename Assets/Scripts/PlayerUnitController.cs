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
    [SerializeField] private Image selectionBox;
    private List<FlockAgent> selected = new List<FlockAgent>();
    private bool isSelectingPosition;
    private bool selectedChecked = false;
    private float moveSetCooldown = 1f;
    private Vector2? selectionStartScreen;
    Vector3 mousePosition = Vector2.zero;
    private void Update()
    {
        if (selectionStartScreen != null)
        {
            selectedChecked = false;
            RectTransform selectionBoxTransform = selectionBox.rectTransform;
            selectionBoxTransform.position = ((Vector3)selectionStartScreen + mousePosition) * 0.5f;
            selectionBoxTransform.sizeDelta = (Vector2)selectionStartScreen - (Vector2)mousePosition;
            selectionBoxTransform.sizeDelta += new Vector2(selectionBoxTransform.sizeDelta.x >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.x * -2.0f, selectionBoxTransform.sizeDelta.y >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.y * -2.0f);
            foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
            selected.Clear();
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.MousePositionWorld(mousePosition));
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
        foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
        if (inputValue.Get<float>() > 0)
        {
            selected.Clear();
            selectionBox.enabled = true;
            selectionStartScreen = CameraControl.Singleton.MousePositionScreen();
            Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen));
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
        isSelectingPosition = inputValue.Get<float>() > 0;
    }
    private void SelectColliders(Collider2D[] collider2Ds)
    {
        foreach (Collider2D collider in collider2Ds)
        {
            if (!FlockAgent.agents.ContainsKey(collider))
            {
                return;
            }
            if (FlockAgent.agents[collider] != null)
            {
                selected.Add(FlockAgent.agents[collider]);
                ((ISelectable)FlockAgent.agents[collider]).SetColour(Color.yellow);
            }
        }
    }
    private void OnMousePosition(InputValue inputValue)
    {
        mousePosition = inputValue.Get<Vector2>();
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
        Flock.SetDestination(CameraControl.Singleton.MousePositionWorld(mousePosition), selected);
        moveSetCooldown = 0.1f;
    }
}
