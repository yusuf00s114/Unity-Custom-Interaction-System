#if CINEMACHINE_PRESENT

using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class LookableDetectorCinemachine : MonoBehaviour
{
    [SerializeField] [Tooltip("What should this object do when the Player looks at something Interactable")]
    public UnityEvent<Lookable> OnLookStart;

    [SerializeField] [Tooltip("What should this object do when the Player stops looking at something Interactable")]
    public UnityEvent<Lookable> OnLookEnd;

    public event Action<Lookable> OnLookStartAction;
    public event Action<Lookable> OnLookEndAction;
    
   // [SerializeField]
    //[Tooltip("The layer of all GameObjects that can be interacted with. By default, this is the \"Lookable\" layer.")]
    //private LayerMask interactablesLayer;

    [SerializeField]
    [Tooltip("The layer of all GameObjects that the raycast can hit & stop at. By default, \"Lookable\" and \"Default\" layers are included.")]
    private LayerMask raycastHitLayers;
    
    
    [SerializeField] [Tooltip("The range at which this initiator can detect and interact with interactables.")]
    private float range = 5f;

    private Lookable _currentLookable;

    /// <summary>
    ///     The GameObject that this initiator is currently looking at.
    /// </summary>
    public GameObject SelectedObject { get; private set; }

    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the interaction range in the Scene view when the object is selected
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        var ray = brain.OutputCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        var collider = Physics.Raycast(ray, out var hit, range, raycastHitLayers) ? hit.collider : null;
        var lookable = collider != null ? collider.gameObject.GetComponent<Lookable>() : null;
        var gameObject = collider != null ? collider.gameObject : null;

        SwitchSelectedObjectTo(gameObject, lookable);

        Debug.DrawLine(ray.origin, ray.direction * range, Color.red);
    }

    private void SwitchSelectedObjectTo(GameObject next, Lookable nextLookable)
    {
        if (SelectedObject == next) return;

        //Debug.Log($"Switching to {next?.name} from Initiative {this.gameObject.name}", this);
        if (_currentLookable != null)
        {
            _currentLookable.OnLookEnd();
            OnLookEnd?.Invoke(_currentLookable);
            OnLookEndAction?.Invoke(_currentLookable);
        }

        _currentLookable = nextLookable;
        SelectedObject = next;
        if (_currentLookable != null)
        {
            _currentLookable.OnLookStart();
            OnLookStart?.Invoke(_currentLookable);
            OnLookStartAction?.Invoke(_currentLookable);
        }
    }
}

#endif