using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BaseDropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private string dropZoneID;

    [SerializeField] protected UnityEvent<BaseDragable> onDroppedCorrect = new UnityEvent<BaseDragable>();
    [SerializeField] protected UnityEvent<BaseDragable> onDroppedIncorrect = new UnityEvent<BaseDragable>();

    public UnityEvent<BaseDragable> OnDroppedCorrect { get { return onDroppedCorrect; } }
    public UnityEvent<BaseDragable> OnDroppedIncorrect { get { return onDroppedIncorrect; } }

    public void OnDrop(PointerEventData eventData)
    {
        if (!CanProceedDrop())
            return;

        if (eventData.pointerDrag == null)
            return;

        BaseDragable dragable = eventData.pointerDrag.GetComponent<BaseDragable>();

        if (dragable == null)
            return;

        bool droppedIsCorrect = dragable.TargetDropZoneID==dropZoneID;

        if (droppedIsCorrect)
        {
            dragable.OnDropCorrect(this);
            DroppedCorrect(dragable);
            onDroppedCorrect?.Invoke(dragable);
        }
        else
        {
            dragable.OnDropIncorrect(this);
            DroppedIncorrect(dragable);
            onDroppedIncorrect?.Invoke(dragable);
        }
    }

    protected virtual bool CanProceedDrop()
    {
        return true;
    }

    protected virtual void DroppedCorrect(BaseDragable dragable)
    {

    }

    protected virtual void DroppedIncorrect(BaseDragable dragable)
    {

    }
}
