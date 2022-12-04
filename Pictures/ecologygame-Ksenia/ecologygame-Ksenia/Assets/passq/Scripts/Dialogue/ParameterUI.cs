using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterUI : MonoBehaviour
{
    [SerializeField] private string parameterName;
    [SerializeField] private Image valueBar;

    [SerializeField] private float effectTime = 0.5f;
    [SerializeField] private LeanTweenType effectType;

    private void Start()
    {
        DialogueParameters.Instance.OnParameterChanged.AddListener(OnValueChanged);

        UpdateRendererInstantly(DialogueParameters.Instance.GetParameter(parameterName));
    }

    private void OnDestroy()
    {
        DialogueParameters.Instance.OnParameterChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(string parameter,float value)
    {
        if (parameter != parameterName)
            return;

        UpdateRenderer(value);
    }

    private void UpdateRendererInstantly(float value)
    {
        float targetValue = value / 100f;

        valueBar.fillAmount = (Mathf.Clamp(targetValue, -1, 1) + 1) / 2f;
    }

    private void UpdateRenderer(float value)
    {
        float targetValue = value / 100f;

        valueBar.gameObject.LeanCancel();

        valueBar.gameObject.LeanValue(valueBar.fillAmount * 2 - 1, targetValue, effectTime).setEase(effectType).setOnUpdate(new System.Action<float>((x) => { valueBar.fillAmount = (Mathf.Clamp(x, -1, 1) + 1) / 2f; }));
    }
}
