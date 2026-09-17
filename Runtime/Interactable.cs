using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YusufShabanov.InteractionSystem;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("What the interactable will do before returning interactionResults")]
    public UnityEvent<GameObject> OnInteract;
    public event Action<GameObject> OnInteractAction;

    [SerializeField] private List<InteractionResult> interactionResults;

    public bool enableInteraction = true;

    public List<InteractionResult> InteractionResults => interactionResults;
    
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
    
    /// <summary>
    /// </summary>
    /// <returns>Null if enableInteraction = false; a List of InteractionResult if enableInteraction = true</returns>
    public List<InteractionResult> Interact(GameObject interactionInitiator)
    {
        if (!enableInteraction) return null;
        OnInteract?.Invoke(interactionInitiator);
        OnInteractAction?.Invoke(interactionInitiator);
        return interactionResults;
    }

    public bool HasInteractionStrategyType<TStrategyType>() where TStrategyType : InteractionStrategyType
    {
        foreach (var result in interactionResults)
        {
            foreach (var t in result.interactionStrategy.InteractionStrategyTypes)
            {
                if (t is TStrategyType)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool HasInteractionStrategyTypeExact<TStrategyType>() where TStrategyType : InteractionStrategyType
    {
        foreach (var result in interactionResults)
        {
            foreach (var t in result.interactionStrategy.InteractionStrategyTypes)
            {
                if (t.GetType() == typeof(TStrategyType))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool HasInteractionType(InteractionType type)
    {
        foreach (var result in interactionResults)
        {
            if (result.interactionStrategy.InteractionTypes.HasFlag(type))
            {
                return true;
            }
        }

        return false;
    }
}