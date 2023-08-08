using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [System.NonSerialized] public Canvas canvas;
    public Material defaultLineMaterial;
    public FlockBehaviour flockBehaviour;
    [SerializeField] private GameObject healthBarPrefab;
    private HealthBar healthBar;
    private void OnEnable()
    {
        Singleton = this;
        canvas = GetComponent<Canvas>();
        healthBar = Instantiate(healthBarPrefab).GetComponent<HealthBar>();
        if (healthBar == null ) { Debug.LogWarning("Health bar not attached to health bar prefab"); }
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        if (!GetHoveredHealthObject(out ITeam teamObjects, out Transform transform))
        {
            healthBar.gameObject.SetActive(false);
            return;
        }
        healthBar.gameObject.SetActive(true);
        healthBar.transform.position = transform.position;
        healthBar.transform.localScale = transform.localScale;
        healthBar.Set(teamObjects.Health / 10);
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
        Collider2D[] hoveredColliders = Physics2D.OverlapPointAll(CameraControl.Singleton.MousePositionWorld());
        for (int i = 0; i < hoveredColliders.Length; i++)
        {
            if (hoveredColliders[i].TryGetComponent(out ITeam teamOwned))
            {
                teamObject = teamOwned;
                transform = hoveredColliders[i].transform;
                return true;
            }
        }
        teamObject = null;
        transform = null;
        return false;
    }
}
