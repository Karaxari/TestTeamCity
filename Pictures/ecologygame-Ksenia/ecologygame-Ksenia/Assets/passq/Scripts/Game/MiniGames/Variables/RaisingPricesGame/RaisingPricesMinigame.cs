using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaisingPricesMinigame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;
    [SerializeField] private Transform pricesContainer;

    private bool isEnabled = false;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        minigameObj.SetActive(true);

        isEnabled = true;
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        minigameObj.SetActive(false);

        isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
            return;

        if (CheckRaisingPrices())
        {
            OnGameEnded();
            EndGame();
        }
    }

    private bool CheckRaisingPrices()
    {
        foreach(Transform child in pricesContainer)
        {
            if(child.GetComponentInChildren<Slider>().value != 1)
            {
                return false;
            }
        }
        return true;
    }
}
