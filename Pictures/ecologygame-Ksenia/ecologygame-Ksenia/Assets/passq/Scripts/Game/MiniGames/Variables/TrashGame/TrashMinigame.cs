using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrashMinigame : MinigameBase
{
    [SerializeField] private GameObject UIPanel;

    [SerializeField] private GameObject trashObj;
    [SerializeField] private List<BaseMapTapable> trashTapables;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private int timeToGame;
    private int currentTime = 0;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        TapController.Instance.SetEnable(true);

        currentTime = timeToGame;

        UIPanel.SetActive(true);
        timeText.text = $"{TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss")}";

        StartCoroutine(GameTimer());

        trashTapables.ForEach(x => x.OnCompleteTapping.AddListener(CheckTrashTapables));
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        StopAllCoroutines();

        trashObj.SetActive(false);
        UIPanel.SetActive(false);

        trashTapables.ForEach(x => x.OnCompleteTapping.RemoveListener(CheckTrashTapables));
    }

    private void CheckTrashTapables()
    {
        if(trashTapables.Where(x=>x.gameObject.activeInHierarchy).All(x=>x.IsCompleted))
            EndGame();
    }

    private IEnumerator GameTimer()
    {
        while(currentTime>0)
        {
            yield return new WaitForSeconds(1);
            currentTime--;
            timeText.text = $"{TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss")}";
        }

        EndGame();
    }
}
