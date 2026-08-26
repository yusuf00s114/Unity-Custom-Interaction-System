using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Lookable))]
public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("What the interactable will do before returning interactionResults")]
    public UnityEvent OnInteract;

    [SerializeField] private List<InteractionResult> interactionResults;

    public bool enableInteraction = true;

    public List<InteractionResult> InteractionResults => interactionResults;
    public bool HasHoldInteraction => CheckForHoldInteraction();
    
    public IReadOnlyList<InteractionStrategy> InteractionStrategies => _interactionStrategies;
    
    private List<InteractionStrategy> _interactionStrategies;

    private void Awake()
    {
        _interactionStrategies = new List<InteractionStrategy>(interactionResults.Count);
        foreach (var r in interactionResults)
        {
            _interactionStrategies.Add(r.interactionStrategy);
        }
    }

    public void SetEnableInteraction(bool value)
    {
        enableInteraction = value;
    }
    
    /// <summary>
    /// </summary>
    /// <returns>Null if enableInteraction = false; a List of InteractionResult if enableInteraction = true</returns>
    public List<InteractionResult> Interact()
    {
        if (!enableInteraction) return null;
        OnInteract?.Invoke();
        return interactionResults;
    }

    private bool CheckForHoldInteraction()
    {
        foreach (var r in InteractionResults)
        {
            if (r.interactionStrategy is HoldInteractionStrategy)
            {
                return true;
            }
        }
        return false;
    }
}