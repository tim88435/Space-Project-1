using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Custom;

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
    private Image selectionBox;
    private Canvas canvas;
    //both are lists because stuff can die
    public List<FlockAgent> selected = new List<FlockAgent>();
    public List<FlockAgent> finalSelected = new List<FlockAgent>();
    public HealthBar healthBar;
    private bool isSelectingPosition;
    private bool selectedChecked = false;
    private int lastCheckedCount = 0;
    private bool specificSelection;
    private float moveSetCooldown = 1f;
    private Vector2? selectionStartScreen;
    private Vector3 mousePositionScreen = Vector2.zero;
    private Color selectionColour = Color.Lerp(Color.cyan, Color.blue, 0.5f);
    private void OnEnable()
    {
        Singleton = this;
        if (canvas == null) canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        if (healthBar == null) { healthBar = Instantiate(GameManager.prefabList.healthBarPrefab).GetComponent<HealthBar>(); }
        if (selectionBox == null) { selectionBox = Instantiate(GameManager.prefabList.selectionBoxPrefab, canvas.transform).GetComponent<Image>(); }
    }
    private void Update()
    {
        if (selectionStartScreen != null)
        {
            UpdateSelectionBox();
            UpdateSelectionHealthBar(selected);
            selected.SetColour(GameManager.Singleton.teamColours[1]);
            selected.ShowHealthBar(false);
            selected.Clear();
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.ScreenPositionToWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.ScreenPositionToWorld(mousePositionScreen), (LayerMask)(1 << 7));
            GetAgents(selectedColliders2D, ref selected);
        }
        else
        {
            healthBar.gameObject.SetActive(false);
        }
        moveSetCooldown -= Time.deltaTime;
        if (isSelectingPosition)
        {
            MoveUnit();
        }
    }

    private void UpdateSelectionHealthBar(List<FlockAgent> selected)
    {
        if (selected.Count == 0)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }
        healthBar.gameObject.SetActive(true);
        Bounds healthBarBounds = GetSelectedBounds(selected);
        healthBarBounds.size *= CameraControl.currentZoom * 0.1f + Mathf.Pow(0.91f, CameraControl.currentZoom);
        healthBarBounds.center += Vector3.down * Mathf.Pow(0.99f, CameraControl.currentZoom);
        healthBar.transform.position = healthBarBounds.center;
        healthBar.transform.localScale = Vector2.one * healthBarBounds.size.magnitude;
        healthBar.Set(selected.Average(x => x.Health / x.MaxHealth));
    }

    private void LateUpdate()
    {
        if (specificSelection)
        {
            finalSelected
                .Where(x => !selected.Contains(x))
                .SetColour(selectionColour)
                .ShowHealthBar(true);
            selected
                .Where(x => !finalSelected.Contains(x))
                .SetColour(Color.yellow)
                .ShowHealthBar(true);
            return;
        }
        selected
            .SetColour(Color.yellow)
            .ShowHealthBar(true);
        finalSelected
            .SetColour(selectionColour)
            .ShowHealthBar(true);
    }
    private void OnSelect(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (UIManager.BuildingSelected)
            {
                DeselectAllFleets();
                return;
            }
            if (UIManager.isHoveringOverUI)
            {
                DeselectAllFleets();
                return;
            }
            selectionBox.enabled = true;
            selectionStartScreen = mousePositionScreen;
            Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.ScreenPositionToWorld((Vector2)selectionStartScreen), (LayerMask)((1 << 6)));
            selected.Clear();
            GetAgents(selectedColliders2D, ref selected);
            return;
        }
        selectionStartScreen = null;
        selectionBox.enabled = false;
        finalSelected
            .SetColour(GameManager.Singleton.teamColours[1])
            .ShowHealthBar(false);
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
    private void OnMove(InputValue inputValue)
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
        Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.ScreenPositionToWorld((Vector2)mousePositionScreen), (LayerMask)~(1 << 6));
        FlockAgent selectedTarget = null;
        for (int i = 0; i < selectedColliders2D.Length; i++)
        {
            if (!FlockAgent.ships.ContainsKey(selectedColliders2D[i]))
            {
                continue;
            }
            if (FlockAgent.ships[selectedColliders2D[i]].TeamID == 1)
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
        Flock.SetDestination(CameraControl.Singleton.ScreenPositionToWorld(mousePositionScreen), finalSelected);
        moveSetCooldown = 0.1f;
        if (selectedTarget != null)
        {
            moveSetCooldown = 1f;
        }
    }
    private void UpdateSelectionBox()
    {
        RectTransform selectionBoxTransform = selectionBox.rectTransform;
        selectionBoxTransform.anchoredPosition = ((Vector3)selectionStartScreen + mousePositionScreen) * 0.5f / canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = ((Vector2)mousePositionScreen - (Vector2)selectionStartScreen) / canvas.scaleFactor;
        selectionBoxTransform.sizeDelta = new Vector2(Mathf.Abs(selectionBoxTransform.sizeDelta.x), Mathf.Abs(selectionBoxTransform.sizeDelta.y));
    }
    private void OnDeselect(InputValue inputValue)
    {
        if (!inputValue.isPressed) { return; }
        DeselectAllFleets();
    }
    private void OnSpecificSelect(InputValue inputValue)
    {
        specificSelection = inputValue.isPressed;
    }
    public static void DeselectAllFleets()
    {
        Singleton.finalSelected
            .SetColour(GameManager.Singleton.teamColours[1])
            .ShowHealthBar(false);
        Singleton.finalSelected.Clear();
        Singleton.selectionStartScreen = null;
        Singleton.selectionBox.enabled = false;
    }
    private Bounds GetSelectedBounds(IEnumerable<FlockAgent> enumerable)
    {
        if (!enumerable.Any())
        {
            return new Bounds(Vector3.zero, Vector3.zero);
        }
        Vector3 firstPosition = enumerable.First().transform.position;
        Vector4 limits = new Vector4(firstPosition.x, firstPosition.x, firstPosition.y, firstPosition.y);
        foreach (FlockAgent agent in enumerable)
        {
            limits.x = Mathf.Min(limits.x, agent.transform.position.x);
            limits.y = Mathf.Max(limits.y, agent.transform.position.x);
            limits.z = Mathf.Min(limits.z, agent.transform.position.y);
            limits.w = Mathf.Max(limits.w, agent.transform.position.y);
        }
        //                                  average mid x               average mid y
        Vector2 centre = new Vector2((limits.x + limits.y / 2), (limits.z + limits.w / 2));
        Vector2 size = new Vector2((centre.x - limits.x), (centre.y - limits.z));
        return new Bounds(centre, size);
    }
    private Bounds GetSelectedBounds(List<FlockAgent> list)
    {
        if (list.Count == 0)
        {
            return new Bounds();
        }
        Vector3 firstPosition = list[0].transform.position;
        Vector4 limits = new Vector4(firstPosition.x, firstPosition.x, firstPosition.y, firstPosition.y);
        for (int i = 0; i < list.Count; i++)
        {
            limits.x = Mathf.Min(limits.x, list[i].transform.position.x);
            limits.y = Mathf.Max(limits.y, list[i].transform.position.x);
            limits.z = Mathf.Min(limits.z, list[i].transform.position.y);
            limits.w = Mathf.Max(limits.w, list[i].transform.position.y);
        }
        //                                  average mid x               average mid y
        Vector2 centre = new Vector2((limits.x + limits.y) / 2, (limits.z + limits.w) / 2);
        Vector2 size = new Vector2(centre.x - limits.x, centre.y - limits.z);
        return new Bounds(centre, size);
    }
}
