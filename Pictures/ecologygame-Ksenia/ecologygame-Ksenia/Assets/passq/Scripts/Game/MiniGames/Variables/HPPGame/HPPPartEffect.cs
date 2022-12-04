using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPPartEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private float yPos;

    [SerializeField] private float effectTime;
    [SerializeField] private LeanTweenType effectType;

    public void Execute()
    {
        gameObject.LeanCancel();

        gameObject.LeanValue(Color.white, new Color32(255, 255, 255, 0), effectTime).setOnUpdate(new System.Action<Color>(x => { renderer.color = x; }));
        gameObject.LeanMoveLocalY(yPos, effectTime).setEase(effectType);
    }
}
