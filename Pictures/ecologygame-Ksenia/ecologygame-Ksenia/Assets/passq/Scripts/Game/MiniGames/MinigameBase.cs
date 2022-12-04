using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBase : MonoBehaviour
{
    [SerializeField] private DialogueElement requiredElement;
    protected virtual void Start()
    {
        DialogueController.Instance.CurrentDialogue.OnDialogueElementChanged.AddListener(OnDialogueChanged);
    }

    private void OnDestroy()
    {
        DialogueController.Instance.CurrentDialogue.OnDialogueElementChanged.RemoveListener(OnDialogueChanged);
    }

    private void OnDialogueChanged(DialogueElement from, DialogueElement to)
    {
        if (to.Type != DialogueElement.DialogueElementType.Minigame)
            return;

        if (to != requiredElement)
            return;

        StartGame();
    }

    private void StartGame()
    {
        OnGameStarted();
    }

    public void EndGame()
    {
        OnGameEnded();

        DialogueController.Instance.SelectNext();
    }

    protected virtual void OnGameStarted()
    {

    }

    protected virtual void OnGameEnded()
    {

    }
}
