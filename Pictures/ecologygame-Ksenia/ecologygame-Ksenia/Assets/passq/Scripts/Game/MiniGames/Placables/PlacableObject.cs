using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlacableObject : MonoBehaviour
{
    [SerializeField] private LayerMask placableZones;
    [SerializeField] private LayerMask blockZones;

    [SerializeField] private UnityEvent onPlaced = new UnityEvent();

    public UnityEvent OnPlaced { get { return onPlaced; } }

    public bool TryPlace()
    {
        if (!CanPlace())
            return false;

        OnPlaced?.Invoke();

        return true;
    }

    private bool CanPlace()
    {
        List<Collider2D> colliders = new List<Collider2D>();

        colliders.AddRange(GetComponents<Collider2D>());
        colliders.AddRange(GetComponentsInChildren<Collider2D>());

        if (!colliders.Any(x => x.IsTouchingLayers(placableZones)))
            return false;

        if (colliders.Any(x => x.IsTouchingLayers(blockZones)))
            return false;

        return true;
    }
}
