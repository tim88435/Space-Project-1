using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
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
    [System.Serializable]
    public struct ResourceData
    {
        public GameObject resourcePrefab;
        [Range(0, 1)] public float chance;
    }
    [HideInInspector] public static PrefabList prefabList;
    public Color[] teamColours = new Color[0];
    public ResourceData[] resourceData;
    public Material defaultLineMaterial;
    public FlockBehaviour flockBehaviour;
    [SerializeField] private float _gameLengthSeconds = 600.0f;
    public static float GameLengthSeconds { get { return Singleton._gameLengthSeconds; } }
    private void OnEnable()
    {
        Singleton = this;
        prefabList = prefabList ?? Resources.Load<PrefabList>("Default Prefab List");
    }
    private void Start()
    {
        Time.timeScale = 0.0f;
        Application.targetFrameRate = 60;
    }
    /*
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
    */
    public Color TeamToColour(ITeam Team)
    {
        return TeamToColour(Team.TeamID);
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
