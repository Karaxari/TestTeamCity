using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameResult
{
    WIN,
    LOSE,
    DRAW
}

//edit. Add realization
public class GameController : MonoBehaviour
{
    DataController dataController;

    #region Mono behaviour
    private void Awake()
    {
        dataController = FindObjectOfType<DataController>();
    }
    private void Start()
    {
        InitGameView();
    }
    #endregion

    #region UI
    private void InitGameView()
    {
        //edit. add realization or delete
    }
    public void OnReturnToMainMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }
    #endregion

    private void StartGame()
    {
    }
    //Called by Invoke. Don't delete
    private void StartNextRound()
    {
    }
    private IEnumerator OnRoundTimeExpired()
    {
        yield return null;
    }
    private void EndGame()
    {
    }
}
