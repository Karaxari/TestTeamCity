using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMoveEffect : Effect
{
    [SerializeField] private Vector3 fromValue, toValue;

    [SerializeField] private float effectTime;
    [SerializeField] private LeanTweenType effectType;

    [SerializeField] private bool startFromDefault = false;
    private Vector3 defaultValue;

    [SerializeField] private bool moveToFromAfter = false;

    private void Awake()
    {
        defaultValue = transform.localPosition;
    }

    public override void Execute()
    {
        Vector3 currentFromValue = fromValue;

        if (startFromDefault)
            currentFromValue = defaultValue;

        transform.localPosition = currentFromValue;

        gameObject.LeanMoveLocal(toValue, effectTime).setEase(effectType).setOnComplete(() =>
        {
            if (moveToFromAfter)
                gameObject.LeanMoveLocal(currentFromValue, effectTime).setEase(effectType);
        });
    }
}
