using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPreprocessor : DialogueElementPreprocessor
{
    [SerializeField] private GameObject obj;
    [SerializeField] private UnityEvent events = new UnityEvent();

    public override void Execute()
    {
        base.Execute();

        events?.Invoke();
    }
}
