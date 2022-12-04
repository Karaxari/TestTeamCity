using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEnabledPreprocessor : DialogueElementPreprocessor
{
    [SerializeField] private string targetItemID;

    [SerializeField] private bool enableItem = false;

    public override void Execute()
    {
        base.Execute();

        GameObject targetObj = DialogueItemID.GetObject(targetItemID);

        if (targetObj == null)
            return;

        targetObj.SetActive(enableItem);
    }
}
