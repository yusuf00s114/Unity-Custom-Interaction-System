using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionStrategy", menuName = "Scriptable Objects/InteractionStrategy")]
[Serializable]
public abstract class InteractionStrategy : ScriptableObject
{
    [Tooltip("A short description of what this strategy does; for developers only--not" +
             " shown in the game or used for any logic.")]
    [SerializeField][TextArea(10, 20)]
    private string description; 
}