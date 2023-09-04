using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Custom.Interfaces;
using Custom.Extensions;
using UnityEngine.EventSystems;

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
    private Canvas canvas; // for scale factor
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
    }
    private void Update()
    {
        if (selectionStartScreen != null)
        {
            UpdateSelectionBox();
            healthBar.gameObject.SetActive(false);
            healthBar.transform.position = CameraControl.Singleton.MousePositionWorld(selectionBox.transform.position);
            healthBar.transform.localScale = Vector3.one * (selected.Count > 0 ? selected : finalSelected).Count;
            selected.SetColour(GameManager.Singleton.teamColours[1]);
            selected.ShowHealthBar(false);
            selected.Clear();
            Collider2D[] selectedColliders2D = Physics2D.OverlapAreaAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (Vector2)CameraControl.Singleton.MousePositionWorld(mousePositionScreen), (LayerMask)(1 << 7));
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
                .SetColour(selectionColour)
                .ShowHealthBar(true);
            return;
        }
        selected.SetColour(Color.yellow)
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
            Collider2D[] selectedColliders2D = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld((Vector2)selectionStartScreen), (LayerMask)((1 << 6)));
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
}
