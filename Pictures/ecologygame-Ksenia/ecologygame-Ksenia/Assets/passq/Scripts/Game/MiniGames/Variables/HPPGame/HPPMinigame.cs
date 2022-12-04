using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPMinigame : MinigameBase
{
    [SerializeField] private GameObject tintObj;

    [SerializeField] private BaseMapTapable tapable;

    [SerializeField] private List<HPPPartEffect> effects = new List<HPPPartEffect>();

    [SerializeField] private GameObject effectsObj;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        TapController.Instance.SetEnable(true);

        tintObj.SetActive(true);

        tapable.OnCompleteTapping.AddListener(EndGame);
        tapable.OnTapped.AddListener(OnTapped);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        tintObj.SetActive(false);

        effectsObj.SetActive(true);

        tapable.OnCompleteTapping.RemoveListener(EndGame);
        tapable.OnTapped.RemoveListener(OnTapped);
    }

    private void OnTapped(int tapID)
    {
        effects[tapID-1].Execute();
    }
}
