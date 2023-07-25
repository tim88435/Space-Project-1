using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUnitController : UnitController
{
    [SerializeField] private Image selectionBox;
    private List<FlockAgent> selected = new List<FlockAgent>();
    private Flock flock;
    private Vector2? selectionStart;
    private bool selectedChecked = false;
    private Vector2 selectionStartScreen;
    private void Update()
    {
        if (selectionStart != null)
        {
            selectedChecked = false;
            RectTransform selectionBoxTransform = selectionBox.rectTransform;
            selectionBoxTransform.position = ((Vector3)selectionStartScreen + CameraControl.Singleton.MousePositionScreen()) * 0.5f;
            selectionBoxTransform.sizeDelta = selectionStartScreen - (Vector2)CameraControl.Singleton.MousePositionScreen();
            selectionBoxTransform.sizeDelta += new Vector2(selectionBoxTransform.sizeDelta.x >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.x * -2.0f, selectionBoxTransform.sizeDelta.y >= 0.0f ? 0.0f : selectionBoxTransform.sizeDelta.y * -2.0f);
            foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
            selected.Clear();
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
        foreach (FlockAgent item in selected) { ((ISelectable)item).SetColour(Color.white); }
        if (inputValue.Get<float>() > 0)
        {
            selected.Clear();
            selectionBox.enabled = true;
            selectionStartScreen = CameraControl.Singleton.MousePositionScreen();
            selectionStart = CameraControl.Singleton.MousePositionWorld();
            return;
        }
        selectionBox.enabled = false;
        for (int i = 0; i < selected.Count; i++)
        {
            ((ISelectable)selected[i]).SetColour(Color.blue);
        }
        selectionStart = null;
    }
    private void OnMoveUnit(InputValue inputValue)
    {
        if (selected.Count <=0)
        {
            return;
        }
        if (!selectedChecked)
        {
            List<FlockAgent> flockToCheck = new List<FlockAgent> ();
            if (flockToCheck == null)
            for (int i = 0; i < selected.Count; i++)
            {

            }
            //var firstNotSecond = selected.Except(flockToCheck).ToList();
            //var secondNotFirst = flockToCheck.Except(selected).ToList();
        }
        for (int i = 0; i < selected.Count; i++)
        {
            selected[i].flock = flock;
        }
        if (inputValue.Get<float>() > 0)
        {
            selected[0].flock.SetDestination(CameraControl.Singleton.MousePositionWorld(), selected);
        }
    }
}
