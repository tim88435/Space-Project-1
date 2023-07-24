using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUnitController : UnitController
{
    [SerializeField] private Image selectionBox;
    private Flock selectedFlock;
    private List<FlockAgent> selected = new List<FlockAgent>();
    private Vector2? selectionStart;
    private Vector2 selectionStartScreen;
    private void Update()
    {
        if (selectionStart != null)
        {
            RectTransform selectionBoxTransform = selectionBox.rectTransform;
            selectionBoxTransform.position = ((Vector3)selectionStartScreen + CameraControl.Singleton.MousePositionScreen()) * 0.5f;
            selectionBoxTransform.sizeDelta = selectionStartScreen - (Vector2)CameraControl.Singleton.MousePositionScreen();
            selectionBoxTransform.sizeDelta += new Vector2(selectionBoxTransform.sizeDelta.x >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.x * -2.0f, selectionBoxTransform.sizeDelta.y >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.y * -2.0f);
            foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(selectionStart.Value, (Vector2)CameraControl.Singleton.MousePositionWorld());
            foreach (Collider2D collider in selectedColliders2D)
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
    }
    private void OnSelect(InputValue inputValue)
    {
        if (selectedFlock != null)
        {
            foreach (ISelectable item in selectedFlock.flockAgents) { item.SetColour(Color.white); }
        }
        selectedFlock = null;
        if (inputValue.Get<float>() > 0)
        {
            selectionBox.enabled = true;
            selectionStartScreen = CameraControl.Singleton.MousePositionScreen();
            selectionStart = CameraControl.Singleton.MousePositionWorld();
            return;
        }
        else
        {
            selectionBox.enabled = false;
        }
        if (selectionStart == null)
        {
            return;
        }
        
        for (int i = 0; i < selected.Count; i++)
        {
            ((ISelectable)selected[i]).SetColour(Color.blue);
        }
        if (selected.Count > 0)
        {
            selectedFlock = new Flock(selected.ToArray());
        }
    }
    private void OnMoveUnit(InputValue inputValue)
    {
        if (selectedFlock == null)
        {
            return;
        }
        if (inputValue.Get<float>() > 0)
        {
            selectedFlock.SetDestination(CameraControl.Singleton.MousePositionWorld());
        }
    }
}
