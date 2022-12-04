using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoringMinigame : MinigameBase
{
    [SerializeField] private GameObject UIPanel;

    [SerializeField] private List<SortingDropZone> dropZones = new List<SortingDropZone>();

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
        if (dropZones.All(x => x.IsTrashcanFull))
        {
            EndGame();
        } //endButton.interactable = true;
    }
}
