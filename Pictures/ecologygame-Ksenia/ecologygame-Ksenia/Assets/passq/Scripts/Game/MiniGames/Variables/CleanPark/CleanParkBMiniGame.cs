using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CleanParkBMiniGame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;

    [Header("First Step")]
    [SerializeField] private GameObject house;
    [SerializeField] ErasingController erasingController;

    [Header("Second Step")]
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private List<CleanParkDropeZone> dropZones = new List<CleanParkDropeZone>();


    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        minigameObj.SetActive(true);

        UIPanel.SetActive(false);

        dropZones.ForEach(x => x.OnDroppedCorrect.AddListener(OnCorrectDropped));

        StartCoroutine(StageProcess());


    }

    private void OnCorrectDropped(BaseDragable dragable)
    {
        if (dropZones.All(x => x.IsLightFixed))
        {
            EndGame();
        }
    }

    IEnumerator StageProcess()
    {
        //Stage1
        house.SetActive(true);

        while (erasingController.crntErasingPicture.stillErasing)
        {
            yield return null;
        }

        //Stage2
        house.SetActive(false);
        UIPanel.SetActive(true);

    }


    protected override void OnGameEnded()
    {
        house.SetActive(false);
        base.OnGameEnded();
    }

}
