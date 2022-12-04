using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadsMinigame : MinigameBase
{
    [SerializeField] private GameObject treesOnRoadObj;

    [SerializeField] private GameObject roadEnvironment;

    [SerializeField] private HoverSlider roadSlider;

    [SerializeField] private GameObject effectsObj;

    [SerializeField] private List<Effect> ecoHumanEffects = new List<Effect>();

    [SerializeField] private List<GameObject> roadElements = new List<GameObject>();

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        treesOnRoadObj.SetActive(false);
        roadEnvironment.SetActive(true);

        roadSlider.OnIndexMoved.AddListener(OnSliderMoved);
        roadSlider.OnMaxIndexArrived.AddListener(OnSliderEnded);

        HoverController.Instance.SetEnable(true);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        roadSlider.gameObject.SetActive(false);

        roadSlider.OnIndexMoved.RemoveListener(OnSliderMoved);
        roadSlider.OnMaxIndexArrived.RemoveListener(OnSliderEnded);

        ecoHumanEffects.ForEach(x => x.Execute());
        effectsObj.SetActive(true);

        HoverController.Instance.SetEnable(false);
    }

    void OnSliderMoved(int index)
    {
        for (int i = 0; i < (index + 1) * 2; i++)
            roadElements[i].SetActive(true);
    }

    void OnSliderEnded()
    {
        EndGame();
    }
}
