using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorEffect : Effect
{
    [SerializeField] private Color32 fromValue, toValue;

    [SerializeField] private float effectTime;
    [SerializeField] private LeanTweenType effectType;

    [SerializeField] private bool startFromDefault = false;
    private Color32 defaultValue;

    [SerializeField] private bool moveToFromAfter = false;

    private SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

        defaultValue = renderer.color;
    }

    public override void Execute()
    {
        Color32 currentFromValue = fromValue;

        if (startFromDefault)
            currentFromValue = defaultValue;

        renderer.color = currentFromValue;

        gameObject.LeanValue(currentFromValue,toValue, effectTime).setEase(effectType).setOnUpdate(new System.Action<Color>(x => { renderer.color = x; })).setOnComplete(() =>
        {
            if (moveToFromAfter)
                gameObject.LeanValue(toValue,currentFromValue, effectTime).setEase(effectType).setOnUpdate(new System.Action<Color>(x => { renderer.color = x; }));
        });
    }
}
