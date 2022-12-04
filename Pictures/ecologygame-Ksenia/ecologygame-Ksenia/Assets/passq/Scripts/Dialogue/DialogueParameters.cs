using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueParameters : MonoBehaviour
{
    public static DialogueParameters Instance { get; private set; }

    private Dictionary<string,float> parameters = new Dictionary<string,float>();

    public readonly UnityEvent<string, float> OnParameterChanged = new UnityEvent<string, float>();

    public readonly UnityEvent<string> OnParameterCreated = new UnityEvent<string>();

    private void Awake()
    {
        Instance = this;
    }

    public float GetParameter(string name)
    {
        CheckParameter(name);

        return parameters[name];
    }

    public void SetParameter(string name,float value)
    {
        CheckParameter(name);

        parameters[name] = value;

        OnParameterChanged?.Invoke(name, value);
    }

    public void AddParameter(string name,float value)
    {
        CheckParameter(name);

        parameters[name] += value;

        OnParameterChanged?.Invoke(name, parameters[name]);
    }

    private void CheckParameter(string name)
    {
        if (parameters.ContainsKey(name))
            return;

        parameters.Add(name, 0f);

        OnParameterCreated?.Invoke(name);
    }
}
