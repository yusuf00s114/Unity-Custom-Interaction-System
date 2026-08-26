using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct InteractionStrategyHandler
{
    [NamedByInteractionStrategy] public InteractionStrategy strategy;
    public UnityEvent<GameObject> response;
}