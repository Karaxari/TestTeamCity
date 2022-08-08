using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormulaController : MonoBehaviour
{
    public int widthMesh = 3;
    public int heightMesh = 2;
    public int colAnswer = 8;
    public int MaxColAnswer = 12;
    public float timerGame = 60;
    public int correctAnswer = 5;
    public int wrongAnswer = 7;
    public int signOperation = 2;
    public int numberTerms = 2;
    public int MAXnumberTerms = 3;
    public int numberValue = 10;
    int score = 0;

    public int level = 0;
    public int easyPoints = 20;
    public int normalPoints = 35;
    public int hardPoints = 50;

    public Transform PanelTerms;
    public Transform PanelOptions;
    public Transform PanelAnswer;
    public Transform PanelExit;
    public Transform PanelEndGame;

    public GameObject PrefabButtonTerms;
    public GameObject PrefabButtonsOptions;

    public Sprite[] arrSpriteOperation;

    List<GameObject> allPanelTerms = new List<GameObject>();
    List<GameObject> allPanelOperands = new List<GameObject>();
    List<GameObject> allPanelOptions = new List<GameObject>();
    GameObject Answer;

    List<List<int>> allTerms = new List<List<int>>();
    List<int> allOperands = new List<int>();
    List<int> allOptions = new List<int>();

    int answer = 0;
    bool gameActive = false;

    DataController dataController;
    public UIController uiController;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        //Debug.Log(gameLevelData.gameLevelConfiguration.gameLevelConfig.Values);
        InitLoadData();
        GenerationMesh();
        Refresh();
        gameActive = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PauseGame();
        }

        if (gameActive)
        {
            timerGame -= Time.deltaTime;
            uiController.outTimer(timerGame);
        }
        
        if(timerGame < 0 && gameActive)
        {
            Debug.Log("EndGame");
            InitResultGame();
            gameActive = false;
            OffOnPanel();
        }
    }
    void InitLoadData()
    {
        //GameSessionData gameSession = new GameSessionData(numberOfPoints);
        var game = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.formula];
        var levelConfig = game["level" + dataController.gameLevelConfiguration.level.ToString()];

        correctAnswer = levelConfig[GameConfig.correctAnswer];
        wrongAnswer = levelConfig[GameConfig.wrongAnswer];
        timerGame = levelConfig[GameConfig.time];
        colAnswer = levelConfig[GameConfig.colAnswer];
        signOperation = levelConfig[GameConfig.sign];
        numberTerms = levelConfig[GameConfig.numberTerms];
        numberValue = levelConfig[GameConfig.numberValue];

        //MAXnumberTerms = numberTerms;
        MaxColAnswer = colAnswer;

        level = levelConfig[GameConfig.level];

        easyPoints = levelConfig[GameConfig.easyPoints];
        normalPoints = levelConfig[GameConfig.normalPoints];
        hardPoints = levelConfig[GameConfig.hardPoints];

        //Debug.Log("Test Load Data: " + easyPoints);
        //Debug.Log(level);
    }

    void InitResultGame()
    {
        if (score > dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].formula)
        {
            GameSessionData gameSession = new GameSessionData(score, GameModes.formula, level);
            dataController.SaveSessionToDatabase(gameSession);
            dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].formula = score;
        }
        if (score >= easyPoints)
        {
            uiController.levelCompleted(score);
            dataController.CheckLevelProgress();
            StartCoroutine(LoadingUserData());
        }
        else
        {
            uiController.levelFailed(score, easyPoints);
        }
    }

    IEnumerator LoadingUserData()
    {
        yield return new WaitForSeconds(0.5f);
        dataController.LoadingUserData();
    }

    void Refresh()
    {
        InitTerms();
        Calculation();
    }

    void GenerationMesh()
    {
        uiController.outScore(score, easyPoints);
        uiController.outTimer(timerGame);


        switch (MAXnumberTerms)
        {
            case 2: heightMesh = 2; widthMesh = 1; break;
            case 3: heightMesh = 3; widthMesh = 2; break;
        }

        PanelTerms.GetComponent<GridLayoutGroup>().constraintCount = heightMesh;

        for (int i = 0; i < heightMesh; i++)
        {
            GameObject point = (GameObject)Instantiate(PrefabButtonTerms, PanelTerms.transform.position, PanelTerms.transform.rotation);
            point.transform.parent = PanelTerms.transform;
            point.name = "Button_" + i.ToString();
            point.transform.localScale = new Vector3(1f, 1f, 1f);
            allPanelTerms.Add(point);
        }

        for (int i = 0; i < widthMesh; i++)
        {
            GameObject point = (GameObject)Instantiate(PrefabButtonTerms, PanelTerms.transform.position, PanelTerms.transform.rotation);
            point.transform.parent = PanelTerms.transform;
            point.GetComponent<Image>().color = Color.white;
            point.name = "Operand_" + i.ToString();
            point.transform.localScale = new Vector3(1f, 1f, 1f);
            allPanelOperands.Add(point);
        }

        PanelOptions.GetComponent<GridLayoutGroup>().constraintCount = MaxColAnswer;

        for (int i = 0; i < MaxColAnswer; i++)
        {
            GameObject button = (GameObject)Instantiate(PrefabButtonsOptions, PanelOptions.transform.position, PanelOptions.transform.rotation);
            button.transform.parent = PanelOptions.transform;
            button.GetComponent<Button>().interactable = false; // делаем просто не активной
            button.name = "Button_" + i.ToString();
            button.SetActive(false);
            button.transform.localScale = new Vector3(1f, 1f, 1f);
            allPanelOptions.Add(button);
        }

        for (int i = 0; i < 2; i++)
        {
            GameObject button = (GameObject)Instantiate(PrefabButtonTerms, PanelAnswer.transform.position, PanelAnswer.transform.rotation);
            button.transform.parent = PanelAnswer.transform;
            button.GetComponent<Button>().interactable = false; // делаем просто не активной
            button.GetComponent<Image>().color = Color.clear;
            button.name = "Answer_" + i.ToString();
            button.transform.localScale = new Vector3(1f, 1f, 1f);

            Answer = button;
        }
        Answer.GetComponent<Image>().color = Color.gray;

        allPanelOperands.Reverse();
        allPanelTerms.Reverse();
    }

    void InitTerms()
    {
        answer = 0;
        allOperands.Clear();
        allOptions.Clear();

        if (allPanelOperands.Count > 1 && allPanelTerms.Count > 1)
        {
            switch (numberTerms)
            {
                case 2: allPanelOperands.Last().SetActive(false); allPanelTerms.Last().SetActive(false);  heightMesh = 2; widthMesh = 1; break;
                case 3: allPanelOperands.Last().SetActive(true); allPanelTerms.Last().SetActive(true); heightMesh = 3; widthMesh = 2; break;
            }
        }
        else Debug.LogError("Empty panel Options and panel Terms and buttons Terms");

        PanelTerms.GetComponent<GridLayoutGroup>().constraintCount = heightMesh;

        for (int i = 0; i < heightMesh; i++)
        {
            allPanelTerms[i].GetComponentInChildren<Text>().text = "";
        }

        for (int i = 0; i < widthMesh; i++)
        {
            int r = Random.Range(0, signOperation);
            allPanelOperands[i].GetComponent<Image>().sprite = arrSpriteOperation[r];
            allOperands.Add(r);
        }

        if (colAnswer <= MaxColAnswer)
        {
            Debug.Log("ColAnswer: " + colAnswer.ToString());

            for (int i = 0; i < colAnswer; i++)
            {
                allPanelOptions[i].gameObject.SetActive(true);

                allPanelOptions[i].GetComponent<Button>().interactable = true;
                allPanelOptions[i].GetComponent<Image>().enabled = true;
                allPanelOptions[i].GetComponentInChildren<Text>().enabled = true;

                int r = Random.Range(0, numberValue);
                allPanelOptions[i].GetComponentInChildren<Text>().text = r.ToString();
                allOptions.Add(r);
            }
            for (int i = colAnswer; i < MaxColAnswer; i++)
            {
                allPanelOptions[i].gameObject.SetActive(false);
            }
        }

        allTerms.Clear();
    }

    void Calculation()
    {
        List<int> indOptions = new List<int>();
        for (int i = 0; i < allOperands.Count; i++)
        {
            while (indOptions.Count <= allOperands.Count)
            {
                int r = Random.Range(0, allOptions.Count);
                if (!indOptions.Contains(r))
                {
                    indOptions.Add(r);
                }
            }
        }

        ///Доработать логику выбора приоритета арифметических операций!!!
        answer = allOptions[indOptions[0]];
        for (int i = 0; i < allOperands.Count; i++)
        {
            switch (allOperands[i])
            {
                case 0: answer += allOptions[indOptions[i + 1]]; break;
                case 1: answer -= allOptions[indOptions[i + 1]]; break;
                case 2: answer *= allOptions[indOptions[i + 1]]; break; 
                case 3: answer /= allOptions[indOptions[i + 1]]; break; //Ошибка деления на ноль подкрадывается
            }
        }

        string str = "";
        for (int i = 0; i < allOperands.Count; i++)
        {
            str += allOptions[indOptions[i]].ToString() + " " + allOperands[i].ToString() + " ";
        }
        str += allOptions[indOptions[allOperands.Count]].ToString();

        Debug.Log(str + " = " + answer.ToString());
        Answer.GetComponentInChildren<Text>().text = answer.ToString();
    }

    void checkResult()
    {
        Debug.Log(allOperands.Count.ToString() + ", " + allTerms.Count.ToString() + "################");
        int result = allTerms[0][2];
        for (int i = 0; i < allOperands.Count; i++)
        {
            switch (allOperands[i])
            {
                case 0: result += allTerms[i + 1][2]; break;
                case 1: result -= allTerms[i + 1][2]; break;
                case 2: result *= allTerms[i + 1][2]; break;
                case 3: result /= allTerms[i + 1][2]; break;
            }
        }

        if (answer == result)
        {
            Debug.Log("Correct Answer!");
            uiController.answerIndicator(true);
            PanelAnswer.GetComponentInChildren<Image>().color = Color.green;
            score += correctAnswer;
        }
        else
        {
            Debug.Log("Wrong Answer!");
            uiController.answerIndicator(false);
            PanelAnswer.GetComponentInChildren<Image>().color = Color.red;
            score -= wrongAnswer;
        }

        uiController.outScore(score, easyPoints);

        //if (score >= normalPoints)
        //{
        //    ScorePanel.text = score.ToString() + "/" + hardPoints.ToString();
        //    ScoreSlider.value = (float)score / (float)hardPoints;
        //}
        //else if (score >= easyPoints)
        //{
        //    ScorePanel.text = score.ToString() + "/" + normalPoints.ToString();
        //    ScoreSlider.value = (float)score / (float)normalPoints;
        //}
        //else
        //{
        //    ScorePanel.text = score.ToString() + "/" + easyPoints.ToString();
        //    ScoreSlider.value = (float)score / (float)easyPoints;
        //}

        StartCoroutine(waitСompletionAnswer());
    }

    IEnumerator waitСompletionAnswer()
    {
        yield return waitAnswer();
        Refresh();
    }
    IEnumerator waitAnswer()
    {
        yield return new WaitForSeconds(0.5f);
        PanelAnswer.GetComponentInChildren<Image>().color = Color.clear;
    }
    public void choiceAnswer(int n)
    {
        if (!gameActive) return;

        int ind = 0;
        for (int i = 0; i < allTerms.Count; i++)
        {
            if(allTerms[i][1] == ind)
            {
                ind++;
            }
        }

        List<int> list = new List<int>();
        list.Add(n);
        list.Add(ind);
        list.Add(allOptions[n]);
        allTerms.Add(list);
        allPanelTerms[ind].GetComponentInChildren<Text>().text = allOptions[n].ToString();

        Debug.Log("Size Term: " + allTerms.Count.ToString());
        if(allTerms.Count > allOperands.Count)
        {
            foreach (List<int> li in allTerms)
            {
                allPanelOptions[li[0]].SetActive(true);
            }
            checkResult();
        }

        visibleTerms(n, false);
        //allPanelOptions[n].SetActive(false);
    }

    public void cancelTerm(int n)
    {
        if (!gameActive) return;

        if (allTerms.Count == 0)
        {
            Debug.Log("Size Term Null : " + allTerms.Count.ToString());
            return;
        }

        if (numberTerms == 2)
        {
            if (allTerms.Count == 1)
            {
                visibleTerms(allTerms[0][0], true);
                //allPanelOptions[allTerms[0][0]].SetActive(true);

                allPanelTerms[allTerms[0][1]].GetComponentInChildren<Text>().text = "";
                allTerms.RemoveAt(0);
            }
        }
        else
        {
            int num = 0;
            foreach (List<int> list in allTerms)
            {
                if (list[1] == ((allPanelTerms.Count - 1) - n))
                {
                    allPanelTerms[list[1]].GetComponentInChildren<Text>().text = "";

                    Debug.Log("Index: " + list[0]);
                    visibleTerms(list[0], true);
                    //1allPanelOptions[list[0]].SetActive(true);

                    num = list[1];
                    break;
                }
            }

            Debug.Log("Num: " + num.ToString());
            Debug.Log("Size Term : " + allTerms.Count.ToString());
            if (num == allTerms.Count - 1)
            {
                allTerms.RemoveAt(num);
                Debug.Log("Size Term Delete: " + allTerms.Count.ToString());
            }
            else
            {
                foreach (List<int> list in allTerms)
                {
                    visibleTerms(list[0], true);
                    //allPanelOptions[list[0]].SetActive(true);

                    allPanelTerms[list[1]].GetComponentInChildren<Text>().text = "";
                }
                allTerms.Clear();
            }
        }
    }

    void visibleTerms(int n, bool key)
    {
        allPanelOptions[n].GetComponent<Button>().interactable = key;
        allPanelOptions[n].GetComponent<Image>().enabled = key;
        allPanelOptions[n].GetComponentInChildren<Text>().enabled = key;
    }

    public void PauseGame()
    {
        uiController.pauseGame(gameActive);
        gameActive = !gameActive;
        OffOnPanel();
    }
    public void ExitGame()
    {
        uiController.quitGame();
    }
    public void OffOnPanel()
    {
        PanelTerms.gameObject.SetActive(gameActive);
        PanelAnswer.gameObject.SetActive(gameActive);
        PanelOptions.gameObject.SetActive(gameActive);
    }
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
