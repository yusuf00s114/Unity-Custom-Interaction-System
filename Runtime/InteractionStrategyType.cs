using UnityEngine;

namespace YusufShabanov.InteractionSystem
{
    [CreateAssetMenu(fileName = "InteractionStrategyType",
        menuName = "Scriptable Objects/Interaction System/InteractionStrategyType")]
    public class InteractionStrategyType : ScriptableObject
    {
        [Tooltip("A short description of what this strategy type is for; for developers only--not" +
                 "shown in the game or used for any logic.")]
        [SerializeField][TextArea(10, 20)]
        private string description;
    }
}
