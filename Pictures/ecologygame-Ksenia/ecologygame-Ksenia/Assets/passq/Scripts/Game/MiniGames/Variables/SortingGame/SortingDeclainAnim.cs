using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingDeclainAnim : MinigameBase
{

    [SerializeField] int countOfExhausting;
    [SerializeField] GameObject factory;
    [SerializeField] GameObject dustClouds;
    [SerializeField] Animator minigamesAnimator;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        factory.SetActive(true);
        StartCoroutine(FactoryExhausting());
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

    }


    IEnumerator FactoryExhausting()
    {
        factory.GetComponentInChildren<ParticleSystem>().Play();
        int i = 0;
        minigamesAnimator.Play("FactoryExhausting");
        while (i < countOfExhausting)
        {
            Debug.Log("говори");
            i++;
            yield return new WaitForSeconds(1f);
        }
        minigamesAnimator.Play("Nothing");

        factory.GetComponentInChildren<ParticleSystem>().Stop();
        dustClouds.SetActive(true);
        factory.SetActive(false);
        EndGame();

    }
}
