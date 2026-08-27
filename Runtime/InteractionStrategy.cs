using System;
using System.Collections.Generic;
using UnityEngine;
using YusufShabanov.InteractionSystem;

[CreateAssetMenu(fileName = "InteractionStrategy",
    menuName = "Scriptable Objects/Interaction System/InteractionStrategy")]
[Serializable]
public class InteractionStrategy : ScriptableObject
{
    [Tooltip("A short description of what this strategy does; for developers only--not" +
             "shown in the game or used for any logic.")]
    [SerializeField][TextArea(10, 20)]
    private string description;
    
    [SerializeField] protected InteractionType interactionTypes;
    [SerializeField] protected List<InteractionStrategyType> interactionStrategyTypes;
    
    public InteractionType InteractionTypes => interactionTypes;
    public IReadOnlyList<InteractionStrategyType> InteractionStrategyTypes => interactionStrategyTypes;
}