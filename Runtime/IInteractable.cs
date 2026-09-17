using System.Collections.Generic;
using UnityEngine;
public interface IInteractable
{
    public List<InteractionResult> Interact(GameObject interactionInitiator);
}