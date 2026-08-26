using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private bool isEnabled = true;
    
    public UnityEvent OnInteractionEnabled;
    public UnityEvent OnInteractionDisabled;

    public IReadOnlyList<InteractionStrategy> InteractionStrategies => _interactionStrategies;
    
    
    
    public bool IsEnabled => isEnabled;
    
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

    public void ToggleInteractionEnabled(bool value)
    {
        //Logger.Log("Setting _isEnabled in InteractionInitiator of " + gameObject.name + " to " + value
        //    , LogChannel.INTERACTION_SYSTEM);
        isEnabled = value;
    }

    /*public void ToggleInteractionEnabled()
    {
        ToggleInteractionEnabled(!_isEnabled);
    }*/

    public void ExecuteInteraction(InteractionResult result, bool respectCooldown = true)
    {
        //Debug.Log("Interaction Cooldown: " + _timer);
        if (result == null) return;
        if (!isEnabled)
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

    /// <summary>
    ///     Executes a list of InteractionResult without waiting for the cooldown.
    /// </summary>
    /// <param name="results"></param>
    public void ExecuteInteraction(List<InteractionResult> results)
    {
        if (results == null) return;
        foreach (var result in results) ExecuteInteraction(result, false);
    }

    /// <summary>
    ///     Executes all HoldInteractionStrategies from a list of InteractionResult without waiting for the cooldown.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="interrupt">
    ///     Null by default. If not null, the interactions will stop being
    ///     executed when the function returns true
    /// </param>
    public void ExecuteHoldInteraction(List<InteractionResult> results, Func<bool> interrupt = null)
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

            if (result.interactionStrategy is HoldInteractionStrategy) ExecuteInteraction(result, false);
        }
    }

    /// <summary>
    ///     Executes all PressInteractionStrategies from a list of InteractionResult without waiting for the cooldown.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="interrupt">
    ///     Null by default. If not null, the interactions will stop being
    ///     executed when the function returns true
    /// </param>
    public void ExecutePressInteraction(List<InteractionResult> results, Func<bool> interrupt = null)
    {
        if (results == null) return;
        foreach (var result in results)
        {
            if (interrupt?.Invoke() == true)
            {
                //Logger.Log("Press interaction execution interrupted before the following was " +
                //           "able to be executed: \n" + result, LogChannel.INTERACTION_SYSTEM);
                return;
            }

            if (result.interactionStrategy is PressInteractionStrategy) ExecuteInteraction(result, false);
        }
    }

    /// <summary>
    ///     Executes all RightPressInteractionStrategies from a list of InteractionResult without waiting for the cooldown.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="interrupt">
    ///     Null by default. If not null, the interactions will stop being
    ///     executed when the function returns true
    /// </param>
    public void ExecuteRightPressInteraction(List<InteractionResult> results, Func<bool> interrupt = null)
    {
        if (results == null) return;
        foreach (var result in results)
        {
            if (interrupt?.Invoke() == true)
            {
                //Logger.Log("Press interaction execution interrupted before the following was " +
                //           "able to be executed: \n" + result, LogChannel.INTERACTION_SYSTEM);
                return;
            }

            if (result.interactionStrategy is RightPressInteractionStrategy) ExecuteInteraction(result, false);
        }
    }
}