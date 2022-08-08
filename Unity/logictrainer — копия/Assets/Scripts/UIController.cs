using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text ScorePanel;
    public Text TimerPanel;
    public Transform PanelPause;
    public Transform PanelQuit;
    public Transform PanelLevelCompleted;
    public Transform PanelLevelFailed;
    public Text scoreComleted;
    public Text scoreFailed;
    public Text scoreWin;
    public Slider ScoreSlider;

    public AnswerIndicatorFading answerController;

    public void outScore(int score, int target)
    {
        ScorePanel.text = score.ToString() + "/" + target.ToString();
        ScoreSlider.value = (float)score / (float)target;
    }

    public void outTimer(float timerGame)
    {
        TimerPanel.text = Mathf.Round(timerGame).ToString();
    }
    
    public void pauseGame(bool active)
    {
        PanelPause.gameObject.SetActive(active);
        if (!active)
        {
            PanelQuit.gameObject.SetActive(active);
        }
    }

    public void answerIndicator(bool result)
    {
        answerController.Show(result);
    }

    public void quitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuSceneDesign");
    }

    public void levelCompleted(int score)
    {
        PanelLevelCompleted.gameObject.SetActive(true);
        scoreComleted.text = score.ToString();
    }

    public void levelFailed(int score, int winScore)
    {
        PanelLevelFailed.gameObject.SetActive(true);
        scoreFailed.text = score.ToString();
        scoreWin.text = winScore.ToString();
    }
}
