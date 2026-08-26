using System;
using UnityEngine;

// Used to add to structs that contain an InteractionStrategy
// so that whenever a list of them is shown, each item will be
// labeled by the InteractionStrategy, and not just "Element 0", "Element 1", etc.

[AttributeUsage(AttributeTargets.Field)]
public class NamedByInteractionStrategyAttribute : PropertyAttribute
{
    
}