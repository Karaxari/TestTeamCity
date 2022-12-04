using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItemsController : MonoBehaviour
{
    private void Start()
    {
        foreach(var item in GameObject.FindObjectsOfType<DialogueItemID>(true))
            item.Initialize();
    }
}
