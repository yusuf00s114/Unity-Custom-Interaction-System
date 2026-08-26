using System;
using UnityEngine;

[Serializable]
public class InteractionResult
{
    [NamedByInteractionStrategy] public InteractionStrategy interactionStrategy;
    public GameObject target;

    public override string ToString()
    {
        return "Interaction Strategy: " + interactionStrategy + "\n" +
               "Target:  " + target;
    }
}