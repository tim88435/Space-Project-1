using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverObject : MonoBehaviour
{
    private static HoverObject _singleton;//does this need to be private? preferable to be public
    public static HoverObject Singleton
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
    public static List<IHoverable> hoveredOver = new List<IHoverable>();
    [SerializeField] private RectTransform hoverbox;
    [SerializeField] private Image rangeIndicator;
    [SerializeField] private Text hoverText;
    //[SerializeField] private HealthBar healthBar;
    private FlockAgent lastSelectedShip;
    private void OnEnable()
    {
        Singleton = this;
    }
    private void Start()
    {
        //if (healthBar == null) { healthBar = Instantiate(GameManager.prefabList.healthBarPrefab, transform).GetComponent<HealthBar>(); }
    }
    private void Update()
    {
        lastSelectedShip?.ShowHealthBar(false);
        if (hoveredOver.Count == 0)
        {
            hoverbox.gameObject.SetActive(false);
            hoverText.gameObject.SetActive(false);
            if (GetHoveredShip(out FlockAgent selectedShip))
            {
                rangeIndicator.gameObject.SetActive(true);
                rangeIndicator.rectTransform.position = CameraControl.Singleton.WorldPositionToScreen(selectedShip.transform.position);
                rangeIndicator.rectTransform.sizeDelta = Vector2.one * (selectedShip.Range / CameraControl.currentZoom) * (Screen.height / rangeIndicator.canvas.scaleFactor);
                if (selectedShip == lastSelectedShip)
                {
                    lastSelectedShip.ShowHealthBar(true);
                    return;
                }
                lastSelectedShip?.ShowHealthBar(false);
                lastSelectedShip = selectedShip;
                lastSelectedShip.ShowHealthBar(true);
                //healthBar.gameObject.SetActive(false);
                return;
            }
            //HealthBarUpdate();
            rangeIndicator.gameObject.SetActive(false);
            return;
        }
        transform.position = CameraControl.Singleton.mousePositionScreen;
        rangeIndicator.gameObject.SetActive(false);
        hoverbox.gameObject.SetActive(true);
        hoverText.gameObject.SetActive(true);
        //healthBar.gameObject.SetActive(false);
        hoverText.text = hoveredOver[0].GetHoverText();
        LayoutRebuilder.ForceRebuildLayoutImmediate(hoverText.rectTransform);
        hoverbox.sizeDelta = hoverText.rectTransform.sizeDelta;
    }
    private bool GetHoveredShip(out FlockAgent ship)
    {
        Collider2D[] hoveredColliders = Physics2D.OverlapPointAll(CameraControl.Singleton.MouseToWorldPosition());
        IEnumerable<FlockAgent> ships = hoveredColliders
            .Where(x => FlockAgent.ships.ContainsKey(x))
            .Select(x => FlockAgent.ships[x]);
        foreach (FlockAgent shipCheck in ships)
        {
            if (shipCheck == lastSelectedShip)
            {
                ship = shipCheck;
                return true;
            }
        }
        ship = ships.FirstOrDefault();
        return ship != null;
    }
    /*
    private void HealthBarUpdate()
    {
        if (UIManager.isHoveringOverUI)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }
        if (!GetHoveredShip(out FlockAgent ship))
        {
            healthBar.gameObject.SetActive(false);
            return;
        }
        healthBar.gameObject.SetActive(true);
        healthBar.transform.position = ship.transform.position;
        //healthBar.transform.localScale = ship.transform.localScale;
        healthBar.Set(ship.Health / ship.MaxHealth);
        healthBar.transform.localScale = Vector3.one * (CameraControl.currentZoom + 1) * 0.1f;
    }
    */
}
