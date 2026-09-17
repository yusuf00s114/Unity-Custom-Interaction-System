using UnityEngine;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Collider))]
public class Lookable : MonoBehaviour, ILookable
{

    [Tooltip("What this object will do when something starts looking at it.")] [SerializeField]
    public UnityEvent OnLookStartBehavior;

    [Tooltip("What this object will do when something stops looking at it.")] [SerializeField]
    public UnityEvent OnLookEndBehavior;

    public event Action OnLookStartAction;
    public event Action OnLookEndAction;

    public bool IsBeingLookedAt { get; private set; }


    public void OnLookStart()
    {
        IsBeingLookedAt = true;
        OnLookStartBehavior?.Invoke();
        OnLookStartAction?.Invoke();
    }

    public void OnLookEnd()
    {
        IsBeingLookedAt = false;
        OnLookEndBehavior?.Invoke();
        OnLookEndAction?.Invoke();
    }
}