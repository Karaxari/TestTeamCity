using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordsDropZone : BaseDropZone
{
    [SerializeField] private GameObject correctObject;
    [SerializeField] private GameObject incorrectObject;

    private bool isCorrectDropped = false;

    public bool IsCorrectDropped { get { return isCorrectDropped; } }

    protected override bool CanProceedDrop()
    {
        return !isCorrectDropped;
    }

    protected override void DroppedCorrect(BaseDragable dragable)
    {
        base.DroppedCorrect(dragable);

        StopAllCoroutines();
        incorrectObject.SetActive(false);

        dragable.gameObject.SetActive(false);
        correctObject.SetActive(true);

        isCorrectDropped = true;
    }

    protected override void DroppedIncorrect(BaseDragable dragable)
    {
        base.DroppedIncorrect(dragable);

        StopAllCoroutines();
        StartCoroutine(ActivateIncorrectObject());
    }

    private IEnumerator ActivateIncorrectObject()
    {
        for(int i=0;i<3;i++)
        {
            incorrectObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            incorrectObject.SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }
    }
}
