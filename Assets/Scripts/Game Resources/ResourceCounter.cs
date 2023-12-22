using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[SelectionBase]
public class ResourceCounter : MonoBehaviour, IHoverableUI
{
    [SerializeField] private Text textComponent;
    [SerializeField] private ResourceType type;

    public string Name { get => type?.name ?? "No Resource"; }
    public string Description { get => type?.description ?? $"No Resource attached to {GetType().Name} on {gameObject.name}"; }

    private void OnEnable()
    {
        if (textComponent == null)
        {
            Debug.LogWarning($"{nameof(textComponent)} is not attached to {GetType()} attached to {gameObject.name}");
            textComponent = GetComponent<Text>();
        }
        if (textComponent == null)
        {
            Debug.LogWarning($"{nameof(Text)} is not attached to {gameObject.name}");
            textComponent = GetComponentInChildren<Text>();
        }
        if (textComponent == null)
        {
            Debug.LogWarning($"{nameof(Text)} is not attached to any of {gameObject.name}'s children");
        }
    }
    private void Update()
    {
        if (type == null)
        {
            return;
        }
        if (textComponent == null)
        {
            return;
        }
        textComponent.text = ((int)ITeamController.teamControllers[1].resources[type]).ToString();
    }
}
