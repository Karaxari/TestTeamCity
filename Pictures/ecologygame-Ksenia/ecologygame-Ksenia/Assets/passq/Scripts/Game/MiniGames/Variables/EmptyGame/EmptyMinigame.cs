using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyMinigame : MinigameBase
{
    [SerializeField] private GameObject objPanel;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        objPanel.SetActive(true);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        objPanel.SetActive(false);
    }
}
