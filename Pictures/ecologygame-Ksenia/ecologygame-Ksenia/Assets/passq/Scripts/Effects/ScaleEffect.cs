using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : Effect
{
    [SerializeField] private Vector3 fromValue, toValue;

    [SerializeField] private float effectTime;
    [SerializeField] private LeanTweenType effectType;

    [SerializeField] private bool startFromDefault = false;
    private Vector3 defaultScale;

    [SerializeField] private bool moveToFromAfter = false;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    public override void Execute()
    {
        Vector3 currentFromValue = fromValue;

        if (startFromDefault)
            currentFromValue = defaultScale;

        transform.localScale = currentFromValue;

        gameObject.LeanScale(toValue, effectTime).setEase(effectType).setOnComplete(() =>
        {
            if (moveToFromAfter)
                gameObject.LeanScale(currentFromValue, effectTime).setEase(effectType);
        });
    }
}
