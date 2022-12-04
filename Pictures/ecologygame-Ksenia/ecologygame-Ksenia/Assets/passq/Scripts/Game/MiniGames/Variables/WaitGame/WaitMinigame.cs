using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitMinigame : MinigameBase
{
    [SerializeField] private float timeToWait = 5f;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        StartCoroutine(WaitProcess());
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        StopAllCoroutines();
    }

    private IEnumerator WaitProcess()
    {
        yield return new WaitForSeconds(timeToWait);

        EndGame();
    }
}
