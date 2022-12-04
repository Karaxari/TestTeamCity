using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFabricMinigame : MinigameBase
{
    [SerializeField] private GameObject tintObj;
    [SerializeField] private GameObject buildingsObj, resourcesObj;

    [SerializeField] private BaseMapTapable resourcesTapable;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        TapController.Instance.SetEnable(true);

        resourcesTapable.OnCompleteTapping.AddListener(EndGame);

        tintObj.SetActive(true);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        resourcesTapable.OnCompleteTapping.RemoveListener(EndGame);

        tintObj.SetActive(false);
        buildingsObj.SetActive(true);
        resourcesObj.SetActive(false);
    }
}
