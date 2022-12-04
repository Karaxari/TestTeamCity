using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingDropZone : BaseDropZone
{
    [SerializeField] private GameObject correctObject;
    [SerializeField] private GameObject incorrectObject;

    private bool trashcanIsFull = false;

    int crntTrashCount;
    [SerializeField] int needTrashCount;

    private void Start()
    {
        crntTrashCount = 0;
    }

    public bool IsTrashcanFull { get { return trashcanIsFull; } }

    protected override bool CanProceedDrop()
    {
        return !trashcanIsFull;
    }

    protected override void DroppedCorrect(BaseDragable dragable)
    {
        crntTrashCount++;
        base.DroppedCorrect(dragable);

        StopAllCoroutines();
        incorrectObject.SetActive(false);

        dragable.gameObject.SetActive(false);
        correctObject.SetActive(true);

        if (crntTrashCount == needTrashCount)
        {
            trashcanIsFull = true;
        }


    }

    protected override void DroppedIncorrect(BaseDragable dragable)
    {
        base.DroppedIncorrect(dragable);

        StopAllCoroutines();
        StartCoroutine(ActivateIncorrectObject());

    }

    private IEnumerator ActivateIncorrectObject()
    {
        for (int i = 0; i < 3; i++)
        {
            incorrectObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            incorrectObject.SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }
    }
}
