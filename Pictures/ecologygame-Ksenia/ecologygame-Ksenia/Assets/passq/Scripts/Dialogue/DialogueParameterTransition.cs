using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DialogueParameterTransition : DialogueTransition
{
    [SerializeField] protected DialogueElement anotherToElement;

    [SerializeField] private string parameterName;
    [SerializeField] private List<ParameterCondition> conditions = new List<ParameterCondition>();

    public DialogueElement AnotherToElement { get { return anotherToElement; } }

    public override bool CanTransit(DialogueElement from, DialogueElement to)
    {
        if (to == toElement)
        {
            if (!base.CanTransit(from, to))
                return false;

            if (!conditions.All(x => x.IsCondition(DialogueParameters.Instance.GetParameter(parameterName))))
                return false;
        }
        else if(to == anotherToElement)
        {
            if (from != fromElement)
                return false;
        }

        return true;
    }

    public override DialogueElement GetNextElement(DialogueElement from)
    {
        if (from != fromElement)
            return null;

        if (CanTransit(from, toElement))
            return toElement;
        else
            return anotherToElement;
    }
}
