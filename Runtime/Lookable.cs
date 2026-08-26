using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using NUnit.Framework;
#endif

[RequireComponent(typeof(Collider))]
public class Lookable : MonoBehaviour, ILookable
{
    // The layer on which all GameObjects that can be interact with live.
    private static int layerIndex;

    [Tooltip("What this object will do when something starts looking at it.")] [SerializeField]
    public UnityEvent OnLookStartBehavior;

    [Tooltip("What this object will do when something stops looking at it.")] [SerializeField]
    public UnityEvent OnLookEndBehavior;

    public bool IsBeingLookedAt { get; private set; }

    // This is called automatically when the script is added
    private void Reset()
    {
        layerIndex = LayerMask.NameToLayer("Lookable");
        gameObject.layer = layerIndex;
    }

    private void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Lookable"))
            Debug.LogWarning("Make sure this GameObject (" + gameObject.name +
                             ") is on the Lookable layer. It is currently on the " + gameObject.layer + " layer");
#if UNITY_EDITOR
        Assert.AreEqual(gameObject.layer, LayerMask.NameToLayer("Lookable"));
#endif
    }

    public void OnLookStart()
    {
        IsBeingLookedAt = true;
        OnLookStartBehavior?.Invoke();
    }

    public void OnLookEnd()
    {
        IsBeingLookedAt = false;
        OnLookEndBehavior?.Invoke();
    }
}