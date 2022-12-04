using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterPreprocessor : DialogueElementPreprocessor
{
    [SerializeField] private string parameterName;
    [SerializeField] private float value = 0;

    public override void Execute()
    {
        base.Execute();

        DialogueParameters.Instance.AddParameter(parameterName, value);
    }
}
