using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanParkDeclainAnim : MinigameBase
{
    [SerializeField] GameObject dustClouds;
  //  [SerializeField] Animator minigamesAnimator;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        dustClouds.SetActive(true);
        EndGame();
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

    }
  
}
