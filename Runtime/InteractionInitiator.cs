using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YusufShabanov.InteractionSystem;

public class InteractionInitiator : MonoBehaviour
{
    
    [SerializeField]
    [Tooltip("In seconds. How many seconds must pass before this InteractionInitiator can" +
             "execute another interaction.\n A value of 0 means there is no cooldown between" +
             "interactions.")]
    private float interactionCooldownSecs;

    [SerializeField]
    [Tooltip(
        "Defines how each interaction strategy will be executed. Strategies that aren't in the list won't be considered or executed.")]
    // maybe this will become a list of ScriptableObjects later
    private List<InteractionStrategyHandler> strategyMappings = new();
    [field: SerializeField] public bool IsEnabled { get; protected set; } = true;
    

    public IReadOnlyList<InteractionStrategy> InteractionStrategies => _interactionStrategies;
    
    // Internal lookup for performance
    private Dictionary<InteractionStrategy, UnityEvent<GameObject>> _lookup;

    private float _timer;
    
    private List<InteractionStrategy> _interactionStrategies;


    private void Awake()
    {
        _timer = interactionCooldownSecs;
        _lookup = new Dictionary<InteractionStrategy, UnityEvent<GameObject>>();
        
        foreach (var mapping in strategyMappings)
            if (mapping.strategy != null)
                _lookup[mapping.strategy] = mapping.response;
        
        _interactionStrategies = new List<InteractionStrategy>(_lookup.Keys);

        if (interactionCooldownSecs <= 0)
        {
            enabled = false;
            Debug.LogWarning("Interaction cooldown for " + name + " is DISABLED.");
        }
        else
        {
            enabled = true;
            Debug.LogWarning("Interaction cooldown for " + name + " is ENABLED.");
        }
    }

    private void Update()
    {
        if (_timer < interactionCooldownSecs) _timer += Time.deltaTime;
        //Debug.Log("Interaction Cooldown: " + _timer);
    }

    /// <summary>
    /// Executes a list of InteractionResult without waiting for the cooldown.
    /// Executes only InteractionResult whose InteractionStrategyType is TStrategyType or a child of TStrategyType,
    /// and whose interaction types include <paramref name="type"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interrupt">
    /// Null by default. If not null, the interactions will stop being
    /// executed when the function returns true.
    /// </param>
    public void ExecuteInteraction<TStrategyType>
    (
        List<InteractionResult> results,
        InteractionType type,
        Func<bool> interrupt = null) 
        where TStrategyType: InteractionStrategyType
    {
        if (results == null) return;
        
        
        foreach (var result in results)
        {
            if (interrupt?.Invoke() == true)
            {
                //Debug.Log("Hold interaction execution interrupted before " + result +
                //          " was able to be executed.");
                return;
            }
            
            foreach (var t in result.interactionStrategy.InteractionStrategyTypes)
            {
                if (t is TStrategyType
                    && result.interactionStrategy.InteractionTypes.HasFlag(type))
                {
                    ExecuteInteraction(result, false);
                    break;
                }
            }

        }
    }

    /// <summary>
    /// Executes a list of InteractionResult without waiting for the cooldown.
    /// Executes only InteractionResult whose InteractionStrategyType is ONLY TStrategyType,
    /// and whose interaction types include <paramref name="type"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interrupt">
    /// Null by default. If not null, the interactions will stop being
    /// executed when the function returns true.
    /// </param>
    public void ExecuteInteractionExact<TStrategyType>(
        List<InteractionResult> results,
        InteractionType type,
        Func<bool> interrupt = null) 
        where TStrategyType : InteractionStrategyType
    {
        if (results == null) return;

        foreach (var result in results)
        {
            if (interrupt?.Invoke() == true)
            {
                //Debug.Log("Hold interaction execution interrupted before " + result +
                //          " was able to be executed.");
                return;
            }
            
            foreach (var t in result.interactionStrategy.InteractionStrategyTypes)
            {
                // Matches exact type only (ignores base classes and derived subclasses)
                if (t.GetType() == typeof(TStrategyType)
                    && result.interactionStrategy.InteractionTypes.HasFlag(type))
                {
                    ExecuteInteraction(result, false);
                    break;
                }
            }

        }
    }

    /// <summary>
    ///     Executes a list of InteractionResult without waiting for the cooldown.
    /// </summary>
    public void ExecuteInteraction(List<InteractionResult> results)
    {
        if (results == null) return;
        foreach (var result in results) ExecuteInteraction(result, false);
    }
    
    public void ExecuteInteraction(InteractionResult result, bool respectCooldown = true)
    {
        //Debug.Log("Interaction Cooldown: " + _timer);
        if (result == null) return;
        if (!IsEnabled)
        {
            //Logger.Log("Interaction initiation is turned off for " + gameObject.name
            //                                                       + " (_isEnabled = false). " +
             //          "Returning from ExecuteInteraction.", LogChannel.INTERACTION_SYSTEM);
            return;
        }
        //Logger.Log("Trying to execute interaction " + result, LogChannel.INTERACTION_SYSTEM);
        if (respectCooldown && _timer < interactionCooldownSecs) return;
        if (_lookup.TryGetValue(result.interactionStrategy, out var unityEvent))
        {
            //Logger.Log("Executing interaction " + result, LogChannel.INTERACTION_SYSTEM);
            unityEvent?.Invoke(result.target);
        }
        else
        {
            //Logger.LogWarning($"{name} has no Inspector handler for {result.interactionStrategy.name}",
            //    LogChannel.INTERACTION_SYSTEM);
        }

        _timer = 0;
    }
    
}