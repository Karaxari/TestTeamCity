using kadyrkaragishiev.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject gamePanel;
    public DialogueScriptableStateMachine sm;
    public DialogueState dialogueState;

    int score;

    public void StartMiniGame()
    {
        gamePanel.SetActive(true);
    }
    public void OnMiniGameEnd()
    {
        //counting score
        foreach (Transform btn in gamePanel.transform)
        {
            score += btn.gameObject.activeInHierarchy ? 1 : 0;
        }

        gamePanel.SetActive(false);

        sm.RequestState(dialogueState);
        Debug.Log("Mini game completed. Your score: " + score);
    }


}
