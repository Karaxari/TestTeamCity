using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseMapHover : MonoBehaviour
{
    [SerializeField] private UnityEvent onHovered = new UnityEvent();
    protected virtual bool CanHover() => true;

    public virtual void OnHover()
    {
        if (!CanHover())
            return;

        onHovered?.Invoke();
    }
}
