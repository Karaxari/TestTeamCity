using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanParkDropeZone : BaseDropZone
{
    [SerializeField] private GameObject correctObject;
    [SerializeField] private GameObject incorrectObject;

    private bool lightIsFixed = false;

    public bool IsLightFixed { get { return lightIsFixed; } }

    protected override bool CanProceedDrop()
    {
        return !lightIsFixed;
    }

    protected override void DroppedCorrect(BaseDragable dragable)
    {

        base.DroppedCorrect(dragable);

        StopAllCoroutines();
        incorrectObject.SetActive(false);

        dragable.gameObject.SetActive(false);
        correctObject.SetActive(true);

        lightIsFixed = true;

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
