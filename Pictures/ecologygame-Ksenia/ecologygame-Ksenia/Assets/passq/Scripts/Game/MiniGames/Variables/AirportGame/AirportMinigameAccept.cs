using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirportMinigameAccept : MinigameBase
{
    [SerializeField] private GameObject treesObj;
    [SerializeField] private GameObject buildingObj;
    [SerializeField] private List<BaseMapTapable> treesTapables;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        buildingObj.SetActive(false);
        TapController.Instance.SetEnable(true);;

        treesTapables.ForEach(x => x.OnCompleteTapping.AddListener(CheckTreesTapables));
    }

    protected override void OnGameEnded()
    {
        buildingObj.SetActive(true);
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        StopAllCoroutines();

        treesObj.SetActive(false);

        treesTapables.ForEach(x => x.OnCompleteTapping.RemoveListener(CheckTreesTapables));
    }

    private void CheckTreesTapables()
    {
        if (treesTapables.Where(x => x.gameObject.activeInHierarchy).All(x => x.IsCompleted))
            EndGame();
    }
}
