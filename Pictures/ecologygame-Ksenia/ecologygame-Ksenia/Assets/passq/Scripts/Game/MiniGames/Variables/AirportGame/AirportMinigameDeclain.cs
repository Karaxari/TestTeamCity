using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportMinigameDeclain : MinigameBase
{
    [SerializeField] private GameObject minigameObj;

    [SerializeField] private GameObject plan;
    [SerializeField] ErasingController erasingController;

    
    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        minigameObj.SetActive(true);

        StartCoroutine(StageProcess());
    }

    IEnumerator StageProcess()
    {

        plan.SetActive(true);

        while (erasingController.crntErasingPicture.stillErasing)
        {
            yield return null;
        }

        plan.SetActive(false);
        EndGame();

    }


    protected override void OnGameEnded()
    {
        base.OnGameEnded();
    }
}
