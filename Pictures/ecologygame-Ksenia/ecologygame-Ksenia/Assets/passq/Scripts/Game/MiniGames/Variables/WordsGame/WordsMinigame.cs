using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordsMinigame : MinigameBase
{
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private Button endButton;

    [SerializeField] private List<WordsDropZone> dropZones = new List<WordsDropZone>();

    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        UIPanel.SetActive(true);

        dropZones.ForEach(x => x.OnDroppedCorrect.AddListener(OnCorrectDropped));
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();
        UIPanel.SetActive(false);

        dropZones.ForEach(x => x.OnDroppedCorrect.RemoveListener(OnCorrectDropped));
    }

    private void OnCorrectDropped(BaseDragable dragable)
    {
        if(dropZones.All(x=>x.IsCorrectDropped))
            endButton.interactable = true;
    }
}
