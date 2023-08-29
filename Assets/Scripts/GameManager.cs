using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;
    public static GameManager Singleton
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
    [System.Serializable]
    public struct ResourceData
    {
        public GameObject resourcePrefab;
        [Range(0, 1)] public float chance;
    }
    public Color[] teamColours = new Color[0];
    public ResourceData[] resourceData;
    [System.NonSerialized] public Canvas canvas;
    public Material defaultLineMaterial;
    public FlockBehaviour flockBehaviour;
    public GameObject healthBarPrefab;
    private HealthBar healthBar;
    [HideInInspector] public bool isHoveringOverUI = false;
    [SerializeField] private float _gameLengthSeconds = 600.0f;
    public static float GameLengthSeconds { get { return Singleton._gameLengthSeconds; } }
    private void OnEnable()
    {
        Singleton = this;
        canvas = GetComponent<Canvas>();
        healthBar = Instantiate(healthBarPrefab).GetComponent<HealthBar>();
        if (healthBar == null ) { Debug.LogWarning("Health bar not attached to health bar prefab"); }
    }
    private void Start()
    {
        Time.timeScale = 0.0f;
        Application.targetFrameRate = 60;
        Resource[] AllResources = Resources.FindObjectsOfTypeAll<Resource>();
        for (int i = 0; i < AllResources.Length; i++)
        {
            AllResources[i].Value = 5.0f;
        }
    }
    private void Update()
    {
        isHoveringOverUI = EventSystem.current.IsPointerOverGameObject();
        if (isHoveringOverUI)
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
    }
    private bool GetHoveredHealthObjects(out ITeam[] teamObjects, out Transform[] transforms)
    {
        Collider2D[] hoveredColliders = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld());
        List<ITeam> teamOwnedList = new List<ITeam>();
        List<Transform> transformsList = new List<Transform>();
        for (int i = 0; i < hoveredColliders.Length; i++)
        {
            if (hoveredColliders[i].TryGetComponent(out ITeam teamOwned))
            {
                transformsList.Add(hoveredColliders[i].transform);
                teamOwnedList.Add(teamOwned);
            }
        }
        teamObjects = teamOwnedList.ToArray();
        transforms = transformsList.ToArray();
        return teamObjects.Length > 0;
    }
    private bool GetHoveredHealthObject(out ITeam teamObject, out Transform transform)
    {
        teamObject = null;
        transform = null;
        Collider2D[] hoveredColliders = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld());
        for (int i = 0; i < hoveredColliders.Length; i++)
        {
            if (hoveredColliders[i].TryGetComponent(out ITeam teamOwned))
            {
                teamObject = teamOwned;
                transform = hoveredColliders[i].transform;
                if (teamOwned as Planet == null)
                {
                    //change maybe? idk
                    return true;
                }
            }
        }
        return teamObject != null;
    }
    private bool GetHoveredShip(out FlockAgent ship)
    {
        Collider2D[] hoveredColliders = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld());
        ship = hoveredColliders
            .Where(x => FlockAgent.ships.ContainsKey(x))
            .Select(x => FlockAgent.ships[x])
            .FirstOrDefault();
        return ship != null;
    }
    public Color TeamToColour(ITeam Team)
    {
        return TeamToColour(Team.Team);
    }
    public Color TeamToColour(int team)
    {
        if (teamColours.Length < team + 1)
        {
            return Color.white;
        }
        return teamColours[team];
    }
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    public void EnableCameraControl(bool enable)
    {
        CameraControl.enableCamera = enable;
    }
}
