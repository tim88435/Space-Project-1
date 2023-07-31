using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIcons : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    private List<RectTransform> currentActiveIcons = new List<RectTransform>();
    private Camera mainCamera;
    [SerializeField] private float minIconViewDistance;
    private void OnEnable()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (mainCamera.orthographicSize < minIconViewDistance)
        {
            for (int i = 0; i < currentActiveIcons.Count; i++)
            {
                Destroy(currentActiveIcons[i].gameObject);
                currentActiveIcons.Remove(currentActiveIcons[i]);
            }
            return;
        }
        for (int i = 0; i < currentActiveIcons.Count; i++)
        {
            UpdateValues(currentActiveIcons[i]);
        }







        //Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.MousePositionWorld(mousePosition));
        //SelectColliders(selectedColliders2D);
    }
    private void UpdateValues(RectTransform rectTransform)
    {

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
                //selected.Add(FlockAgent.agents[collider]);
                //((ISelectable)FlockAgent.agents[collider]).SetColour(Color.yellow);
            }
        }
    }
}
