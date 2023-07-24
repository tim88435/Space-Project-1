using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitController : UnitController
{

    private Flock selected;
    private Vector2? selectionStart;
    private void OnSelect(InputValue inputValue)
    {
        selected = null;
        if (inputValue.Get<float>() > 0)
        {
            selectionStart = CameraControl.Singleton.MousePositionWorld();
            return;
        }
        if (selectionStart == null)
        {
            return;
        }
        Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(selectionStart.Value, (Vector2)CameraControl.Singleton.MousePositionWorld());
        List<FlockAgent> selectedAgents = new List<FlockAgent>();
        foreach (Collider2D collider in selectedColliders2D)
        {
            if (FlockAgent.agents.ContainsKey(collider))
            {
                selectedAgents.Add(FlockAgent.agents[collider]);
            }
        }
        if (selectedAgents.Count > 0)
        {
            selected = new Flock(selectedAgents.ToArray());
        }
    }
    private void OnMoveUnit(InputValue inputValue)
    {
        if (selected == null)
        {
            return;
        }
        if (inputValue.Get<float>() > 0)
        {
            selected.SetDestination(CameraControl.Singleton.MousePositionWorld());
        }
    }
}
