using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueTransition
{
    [SerializeField] protected DialogueElement fromElement;
    [SerializeField] protected DialogueElement toElement;

    public DialogueElement FromElement { get { return fromElement; } }
    public DialogueElement ToElement { get { return toElement; } }

    public virtual bool CanTransit(DialogueElement from, DialogueElement to) => fromElement == from && toElement == to;

    public virtual DialogueElement GetNextElement(DialogueElement from) => fromElement==from? toElement : null;
}
