using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ParameterAction : MonoBehaviour
{
    [SerializeField] private string parameterName;

    [SerializeField] private List<ParameterCondition> conditions = new List<ParameterCondition>();

    [SerializeField] private UnityEvent OnConditionsPassed = new UnityEvent();
    [SerializeField] private UnityEvent OnConditionsFailed = new UnityEvent();

    private void Start()
    {
        DialogueParameters.Instance.OnParameterChanged.AddListener(OnParameterChanged);

        CheckParameter(DialogueParameters.Instance.GetParameter(parameterName));
    }

    private void OnDestroy()
    {
        DialogueParameters.Instance.OnParameterChanged.RemoveListener(OnParameterChanged);
    }

    void OnParameterChanged(string parameter,float value)
    {
        if (parameter != parameterName)
            return;

        CheckParameter(value);
    }

    private void CheckParameter(float value)
    {
        if (conditions.All(x => x.IsCondition(value)))
            OnConditionsPassed?.Invoke();
        else
            OnConditionsFailed?.Invoke();
    }
}

[System.Serializable]
public class ParameterCondition
{
    public enum ParameterActionConditionType
    {
        Less,
        Greater,
        LessOrEuqals,
        GreaterOrEquals,
        Equals
    }

    [SerializeField] private ParameterActionConditionType conditionType;
    [SerializeField] private float targetValue;

    public bool IsCondition(float value)
    {
        if (conditionType == ParameterActionConditionType.Less && value < targetValue)
            return true;

        if (conditionType == ParameterActionConditionType.Greater && value > targetValue)
            return true;

        if (conditionType == ParameterActionConditionType.LessOrEuqals && value <= targetValue)
            return true;

        if (conditionType == ParameterActionConditionType.GreaterOrEquals && value >= targetValue)
            return true;

        if (conditionType == ParameterActionConditionType.Equals && value == targetValue)
            return true;

        return false;
    }
}
