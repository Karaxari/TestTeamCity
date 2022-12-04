using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingFoodGame : MinigameBase
{
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private Transform Container;

    [SerializeField] private Image LeftIndicator;
    [SerializeField] private Image RightIndicator;

    private int count;
    private float startCount;
    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        UIPanel.SetActive(true);
        count = Container.childCount;
        startCount = count;
    }

    public void PickUpFruit()
    {
        count--;

        float indicator = ((float)count / startCount);
        Debug.LogError(indicator);

        LeftIndicator.fillAmount = indicator;
        RightIndicator.fillAmount = indicator;

        if (count <= 1)
        {
            UIPanel.SetActive(false);
            EndGame();
        }
    }
}
