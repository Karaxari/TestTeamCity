using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public class BaseDragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Transform dragParent;

    [SerializeField] private string targetDropZoneID;

    private Transform mainParent;
    private Vector2 mainPos;

    private Vector2 grabOffset = Vector2.zero;

    private List<Graphic> allraycastListeners = new List<Graphic>();

    public string TargetDropZoneID { get { return targetDropZoneID; } }

    protected virtual void Start()
    {
        mainParent = transform.parent;
        mainPos = transform.localPosition;

        allraycastListeners.AddRange(GetComponents<Graphic>().Where(x=>x.raycastTarget));
        allraycastListeners.AddRange(GetComponentsInChildren<Graphic>().Where(x => x.raycastTarget));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        grabOffset = transform.position - Input.mousePosition;

        transform.SetParent(dragParent);

        allraycastListeners.ForEach(x => x.raycastTarget = false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector2)Input.mousePosition + grabOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetDragable();
    }

    public virtual void OnDropCorrect(BaseDropZone zone)
    {
        ResetDragable();
    }

    public virtual void OnDropIncorrect(BaseDropZone zone)
    {
        ResetDragable();
    }

    protected void ResetDragable()
    {
        allraycastListeners.ForEach(x => x.raycastTarget = true);

        transform.SetParent(mainParent);
        transform.localPosition = mainPos;
    }
}
