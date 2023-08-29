using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DestroyBuilding : MonoBehaviour, IHoverable
{
    public string Name { get; private set; } = "Destroy Target Building";
    public string Description { get; private set; } = "No refunds";
    private static DestroyBuilding singleton;
    public static DestroyBuilding Singleton
    {
        get { return singleton; }
        set
        {
            if (singleton == null)
            {
                singleton = value;
                return;
            }
            if (singleton != value)
            {
                Destroy(value);
                return;
            }
        }
    }
    private void OnEnable()
    {
        Singleton = this;
        UIManager.Singleton.hoveredOver.Add(this);
    }
    private void OnDisable()
    {
        UIManager.Singleton.hoveredOver.Remove(this);
    }
    public void Update()
    {
        Building selectedBuilding = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld())
            .Select(x => x.GetComponent<Building>())
            .Where(x => x != null)
            .FirstOrDefault(x => x.Team == 1);
        if (selectedBuilding == null)
        {
            Name = "Destroy Target Building";
            Description = "No Building Selected";
            return;
        }
        Name = $"Destroy {selectedBuilding.name}";
        Description = $"No refunds";
    }
    private void OnSelect(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        Building selectedBuilding = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld())
            .Select(x => x.GetComponent<Building>())
            .Where(x => x != null)
            .FirstOrDefault(x => x.Team == 1);
        if (selectedBuilding == null)
        {
            if (!UIManager.multiplePlace)
            {
                gameObject.SetActive(false);
            }
            return;
        }
        Destroy(selectedBuilding.gameObject);
        selectedBuilding.transform.parent.GetComponent<Planet>().buildings.Remove(selectedBuilding);
        if (UIManager.multiplePlace)
        {
            return;
        }
        gameObject.SetActive(false);
        Name = "Destroy Target Building";
        Description = "No refunds";
    }
    private void OnMousePosition(InputValue inputValue)
    {
        transform.position = CameraControl.Singleton.MousePositionScreen();
    }
    private void OnMove(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        gameObject.SetActive(false);
        Name = "Destroy Target Building";
        Description = "No refunds";
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //use enable and disable instead
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //use enable and disable instead
    }
}
