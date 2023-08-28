using Custom.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[SelectionBase]
public class ResourceCounter : MonoBehaviour, IHoverable
{
    [SerializeField] private Text textComponent;
    [SerializeField] private Resource type;

    public string Name { get => type?.name ?? "Unnamed Resource"; }
    public string Description { get => type?.Description ?? "Describled Resource"; }

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
        textComponent.text = ((int)type.Value).ToString();
    }
}
