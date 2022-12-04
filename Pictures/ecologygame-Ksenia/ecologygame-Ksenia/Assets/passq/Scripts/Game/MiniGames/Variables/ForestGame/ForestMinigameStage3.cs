using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForestMinigameStage3 : MinigameBase
{
    [SerializeField] private List<BaseMapTapable> workerTapables = new List<BaseMapTapable>();

    [SerializeField] private GameObject effectsObj;

    [SerializeField] private GameObject tintObj;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        TapController.Instance.SetEnable(true);

        workerTapables.ForEach(x => x.OnCompleteTapping.AddListener(OnWorkerTapped));

        tintObj.SetActive(true);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        workerTapables.ForEach(x => x.OnCompleteTapping.RemoveListener(OnWorkerTapped));

        tintObj.SetActive(false);

        effectsObj.SetActive(true);
    }

    private void OnWorkerTapped()
    {
        if(workerTapables.All(x=>x.IsCompleted))
            EndGame();
    }
}
