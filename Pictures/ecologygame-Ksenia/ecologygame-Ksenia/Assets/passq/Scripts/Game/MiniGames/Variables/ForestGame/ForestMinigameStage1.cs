using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestMinigameStage1 : MinigameBase
{
    [SerializeField] private List<Effect> workerEffects = new List<Effect>();
    [SerializeField] private List<Effect> treeEffects = new List<Effect>();

    [SerializeField] private float timeToWait = 3f;
    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        workerEffects.ForEach(effect => effect.Execute());

        StartCoroutine(ProcessCoroutine());
    }

    private IEnumerator ProcessCoroutine()
    {
        for(int i=0;i<treeEffects.Count;i++)
        {
            yield return new WaitForSeconds((timeToWait - 1) / treeEffects.Count);
            treeEffects[i].Execute();
        }

        yield return new WaitForSeconds(1);

        EndGame();
    }
}