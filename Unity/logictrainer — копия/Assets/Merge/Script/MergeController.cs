using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//#refactor. Разделить UI часть от логики генерации заданий, проверки ответов и т.д. Можно выделить под это отдельный объект ViewController с соответствующим классом
public class MergeController : MonoBehaviour
{
    public int MAXheightMesh = 3;
    public int MAXwidthMesh = 2;
    int heightMesh = 3;
    int widthMesh = 2;
    public int MAXsizePanelTerms = 6;
    public int sizePanelTerms = 3; // sizeMesh - размер матриц используемых в операциях (не более 6)
    public int colAnswer = 4;
    public int MaxColAnswer = 8;

    public float timerGame = 60;
    public int correctAnswer = 5;
    public int wrongAnswer = 7;
    public int numberTerms = 2;
    int score = 0;

    public int level = 0;
    public int easyPoints = 20;
    public int normalPoints = 35;
    public int hardPoints = 50;

    public Text ScorePanel;
    public Text TimerPanel;

    public Transform Panel; //#refactor. Лучше не называть объекты словами "Panel", "GameObject" и т.д., так как не дает понимания, какова роль объекта (хотя в каких-то случая думаю допустимо)
    public Transform PanelAnswers;
    public Transform PanelButtons;
    public Transform PanelExit;
    public Transform PanelEndGame;
    public GameObject PrefabPanel;
    public GameObject PrefabCube;
    public GameObject PrefabButton;

    public Sprite Plus;
    public Sprite Minus;

    List<GameObject> allPanelTerms = new List<GameObject>();
    List<List<GameObject>> allPanelTermsCubes = new List<List<GameObject>>();
    List<GameObject> allPanelOperands = new List<GameObject>();
    List<GameObject> allPanelAnswers = new List<GameObject>();
    List<List<GameObject>> allPanelAnswersCubes = new List<List<GameObject>>();
    List<GameObject> allPanelButtonsAnswers = new List<GameObject>();

    List<int[,]> allTerms = new List<int[,]>();
    List<int> allOperands = new List<int>();
    List<int[,]> allAnswer = new List<int[,]>();

    bool gameActive = false;

    DataController dataController;
    public UIController uiController;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        InitLoadData();

        GenerationMesh();
        Refresh();
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

        if (timerGame < 0 && gameActive)
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
        var game = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.merge];
        var levelConfig = game["level" + dataController.gameLevelConfiguration.level.ToString()];

        correctAnswer = levelConfig[GameConfig.correctAnswer];
        wrongAnswer = levelConfig[GameConfig.wrongAnswer];
        timerGame = levelConfig[GameConfig.time];
        colAnswer = levelConfig[GameConfig.colAnswer];
        sizePanelTerms = levelConfig[GameConfig.sizeMesh];
        numberTerms = levelConfig[GameConfig.numberTerms];

        MAXsizePanelTerms = sizePanelTerms;
        MaxColAnswer = colAnswer;

        level = levelConfig[GameConfig.level];

        easyPoints = levelConfig[GameConfig.easyPoints];
        normalPoints = levelConfig[GameConfig.normalPoints];
        hardPoints = levelConfig[GameConfig.hardPoints];

        Debug.Log("Test Load Data: " + easyPoints);
        //Debug.Log(level);
    }

    void InitResultGame()
    {
        if (score > dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].merge)
        {
            GameSessionData gameSession = new GameSessionData(score, GameModes.merge, level);
            dataController.SaveSessionToDatabase(gameSession);
            dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].merge = score;
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
        gameActive = true;
        
        allTerms.Clear();
        allOperands.Clear();
        allAnswer.Clear();

        //sizePanelTerms = Random.Range(1, MAXsizePanelTerms + 1);

        GenerationTerms();
        InitTerms();
        Calculation();
    }

    void GenerationMesh()
    {
        uiController.outScore(score, easyPoints);
        uiController.outTimer(timerGame);

        Panel.GetComponent<GridLayoutGroup>().constraintCount = MAXheightMesh;

        for (int k = 0; k < MAXheightMesh; k++)
        {
            GameObject point = (GameObject)Instantiate(PrefabPanel, Panel.transform.position, Panel.transform.rotation);
            point.transform.parent = Panel.transform;
            point.AddComponent<GridLayoutGroup>();
            InitGridLayoutGroup(point.GetComponent<GridLayoutGroup>(), MAXsizePanelTerms);
            //var grid = point.GetComponent<GridLayoutGroup>();
            ////point.GetComponent<GridLayoutGroup>().padding.left = 5;
            ////point.GetComponent<GridLayoutGroup>().padding.right = 5;
            ////point.GetComponent<GridLayoutGroup>().padding.top = 5;
            ////point.GetComponent<GridLayoutGroup>().padding.bottom = 5;
            //grid.cellSize = new Vector2((160f / sizePanelTerms), (160f / sizePanelTerms));
            //grid.spacing = new Vector2(30f / sizePanelTerms, 30f / sizePanelTerms);
            //grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            //grid.childAlignment = TextAnchor.MiddleCenter;
            //grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            //grid.constraintCount = sizePanelTerms;
            List<GameObject> arr = new List<GameObject>();
            for (int i = 0; i < MAXsizePanelTerms; i++)
            {
                for (int j = 0; j < MAXsizePanelTerms; j++)
                {
                    GameObject cube = (GameObject)Instantiate(PrefabCube, point.transform.position, point.transform.rotation);
                    cube.transform.parent = point.transform;
                    cube.GetComponentInChildren<Image>().color = Color.yellow;
                    //cube.AddComponent<BoxCollider2D>();
                    //arr[i, j] = 0;
                    //if (Random.Range(0, 2) == 0)
                    //{
                    //    cube.GetComponentInChildren<Image>().color = Color.red;
                    //    arr[i, j] = 1;
                    //}
                    cube.name = "Cube_" + ((i * MAXsizePanelTerms) + j).ToString();
                    cube.transform.localScale = new Vector3((1.6f / MAXsizePanelTerms), (1.6f / MAXsizePanelTerms), (1.6f / MAXsizePanelTerms));
                    arr.Add(cube);
                }
            }


            point.name = "Term_" + k.ToString();
            point.transform.localScale = new Vector3(1f, 1f, 1f);
            allPanelTerms.Add(point);
            allPanelTermsCubes.Add(arr);
        }

        for (int i = 0; i < MAXwidthMesh; i++)
        {
            GameObject point = (GameObject)Instantiate(PrefabPanel, Panel.transform.position, Panel.transform.rotation);
            point.transform.parent = Panel.transform;
            point.name = "Operand_" + i.ToString();
            point.GetComponentInChildren<Image>().color = Color.white;
            point.transform.localScale = new Vector3(1f, 1f, 1f);
            allPanelOperands.Add(point);
        }

        PanelAnswers.GetComponent<GridLayoutGroup>().constraintCount = MaxColAnswer;
        PanelButtons.GetComponent<GridLayoutGroup>().constraintCount = MaxColAnswer;
        //PrefabButton.GetComponent<Image>().color = Color.clear;

        for (int k = 0; k < MaxColAnswer; k++)
        {
            GameObject point = (GameObject)Instantiate(PrefabPanel, PanelAnswers.transform.position, PanelAnswers.transform.rotation);
            point.transform.parent = PanelAnswers.transform;
            point.AddComponent<GridLayoutGroup>();
            InitGridLayoutGroup(point.GetComponent<GridLayoutGroup>(), MAXsizePanelTerms);
            point.name = "Answer_" + k.ToString();

            List<GameObject> arr = new List<GameObject>();
            for (int i = 0; i < MAXsizePanelTerms; i++)
            {
                for (int j = 0; j < MAXsizePanelTerms; j++)
                {
                    GameObject cube = (GameObject)Instantiate(PrefabCube, point.transform.position, point.transform.rotation);
                    cube.transform.parent = point.transform;
                    cube.GetComponentInChildren<Image>().color = Color.yellow;
                    //cube.AddComponent<BoxCollider2D>();
                    //arr[i, j] = 0;
                    //if (Random.Range(0, 2) == 0)
                    //{
                    //    cube.GetComponentInChildren<Image>().color = Color.red;
                    //    arr[i, j] = 1;
                    //}
                    cube.name = "Cube_" + ((i * MAXsizePanelTerms) + j).ToString();
                    cube.transform.localScale = new Vector3((1.6f / MAXsizePanelTerms), (1.6f / MAXsizePanelTerms), (1.6f / MAXsizePanelTerms));
                    cube.SetActive(false);
                    arr.Add(cube);
                }
            }
            point.transform.localScale = new Vector3(1f, 1f, 1f);

            allPanelAnswers.Add(point);
            allPanelAnswersCubes.Add(arr);

            GameObject button = (GameObject)Instantiate(PrefabButton, PanelButtons.transform.position, PanelButtons.transform.rotation);
            button.transform.parent = PanelButtons.transform;
            //button.AddComponent<RectTransform>();
            //button.AddComponent<Button>();
            button.GetComponent<Image>().color = Color.clear;
            //button.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 200f); //SetSize(new Vector2(200f, 200f));
            button.name = "Button_" + k.ToString();
            button.transform.localScale = new Vector3(1f, 1f, 1f);
            //allPanelAnswer.Add(point);
            allPanelButtonsAnswers.Add(button);
        }
    }


    //#refactor. Код метода не изучал, но выглядит большеватым. Если есть возможность/идеи как можно сократить код (например, разбив на методы), было бы хорошо
    void GenerationTerms()
    {
        switch (numberTerms)
        {
            case 2: heightMesh = 2; widthMesh = 1; break;
            case 3: heightMesh = 3; widthMesh = 2; break;
        }

        Panel.GetComponent<GridLayoutGroup>().constraintCount = heightMesh;

        //-------------------------------------------------------------------------------
        for (int k = 0; k < heightMesh; k++)
        {
            allPanelTerms[k].SetActive(true);
            InitGridLayoutGroup(allPanelTerms[k].GetComponent<GridLayoutGroup>(), sizePanelTerms);
            int[,] arr = new int[sizePanelTerms, sizePanelTerms];
            for (int i = 0; i < sizePanelTerms; i++)
            {
                for (int j = 0; j < sizePanelTerms; j++)
                {
                    int ind = (i * sizePanelTerms) + j;
                    allPanelTermsCubes[k][ind].SetActive(true);
                    allPanelTermsCubes[k][ind].GetComponentInChildren<Image>().color = Color.clear;
                    //cube.AddComponent<BoxCollider2D>();
                    arr[i, j] = 0;
                    if (Random.Range(0, 2) == 0)
                    {
                        allPanelTermsCubes[k][ind].GetComponentInChildren<Image>().color = Color.red;
                        arr[i, j] = 1;
                    }
                    //allPanelAnswersCubes[k][i, j].name = "Cube_" + ((i * sizePanelTerms) + j).ToString();
                    allPanelTermsCubes[k][ind].transform.localScale = new Vector3((1.6f / sizePanelTerms), (1.6f / sizePanelTerms), (1.6f / sizePanelTerms));
                }
            }
            allTerms.Add(arr);

            for (int i = arr.Length; i < MAXsizePanelTerms * MAXsizePanelTerms; i++)
            {
                allPanelTermsCubes[k][i].GetComponentInChildren<Image>().color = Color.clear;
                allPanelTermsCubes[k][i].SetActive(false);
            }
        }
        for (int k = heightMesh; k < MAXheightMesh; k++)
        {
            allPanelTerms[k].SetActive(false);
        }
        //-------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------
        for (int k = 0; k < widthMesh; k++)
        {
            allPanelOperands[k].SetActive(true);
            int n = 0;
            allPanelOperands[k].GetComponent<Image>().sprite = Plus;
            if (Random.Range(0, 2) == 0)
            {
                allPanelOperands[k].GetComponent<Image>().sprite = Minus;
                n = 1;
            }
            allOperands.Add(n);
        }
        for (int k = widthMesh; k < MAXwidthMesh; k++)
        {
            allPanelOperands[k].SetActive(false);
        }
        //-------------------------------------------------------------------------------

        PanelAnswers.GetComponent<GridLayoutGroup>().constraintCount = colAnswer;
        PanelButtons.GetComponent<GridLayoutGroup>().constraintCount = colAnswer;
        //-------------------------------------------------------------------------------
        for (int k = 0; k < colAnswer; k++)
        {
            allPanelAnswers[k].SetActive(true);
            allPanelButtonsAnswers[k].SetActive(true);
            InitGridLayoutGroup(allPanelAnswers[k].GetComponent<GridLayoutGroup>(), sizePanelTerms);
            int[,] arr = new int[sizePanelTerms, sizePanelTerms];
            for (int i = 0; i < sizePanelTerms; i++)
            {
                for (int j = 0; j < sizePanelTerms; j++)
                {
                    int ind = (i * sizePanelTerms) + j;
                    allPanelAnswersCubes[k][ind].SetActive(true);
                    allPanelAnswersCubes[k][ind].GetComponentInChildren<Image>().color = Color.clear;
                    //cube.AddComponent<BoxCollider2D>();
                    arr[i, j] = 0;
                    if (Random.Range(0, 2) == 0)
                    {
                        allPanelAnswersCubes[k][ind].GetComponentInChildren<Image>().color = Color.red;
                        arr[i, j] = 1;
                    }
                    //allPanelAnswersCubes[k][i, j].name = "Cube_" + ((i * sizePanelTerms) + j).ToString();
                    allPanelAnswersCubes[k][ind].transform.localScale = new Vector3((1.6f / sizePanelTerms), (1.6f / sizePanelTerms), (1.6f / sizePanelTerms));
                }
            }
            //allPanelAnswers[k].GetComponent<Image>().color = Color.blue;
            allAnswer.Add(arr);

            for (int i = arr.Length; i < MAXsizePanelTerms * MAXsizePanelTerms; i++)
            {
                allPanelAnswersCubes[k][i].GetComponentInChildren<Image>().color = Color.clear;
                allPanelAnswersCubes[k][i].SetActive(false);
            }
        }
        for (int k = colAnswer; k < MaxColAnswer; k++)
        {
            allPanelAnswers[k].SetActive(false);
            allPanelButtonsAnswers[k].SetActive(false);
        }
        //-------------------------------------------------------------------------------

        allTerms.Reverse();
        allOperands.Reverse();

        //allPanelTermsCubes.Reverse();

        //Debug.Log("---------------------------------------------");

        //Debug.Log(allTerms[0][0, 0] + " " + allTerms[0][0, 1] + " " + allTerms[0][0, 2]);
        //Debug.Log(allTerms[0][1, 0] + " " + allTerms[0][1, 1] + " " + allTerms[0][1, 2]);
        //Debug.Log(allTerms[0][2, 0] + " " + allTerms[0][2, 1] + " " + allTerms[0][2, 2]);

        //Debug.Log(allTerms[1][0, 0] + " " + allTerms[1][0, 1] + " " + allTerms[1][0, 2]);
        //Debug.Log(allTerms[1][1, 0] + " " + allTerms[1][1, 1] + " " + allTerms[1][1, 2]);
        //Debug.Log(allTerms[1][2, 0] + " " + allTerms[1][2, 1] + " " + allTerms[1][2, 2]);

        //if (heightMesh == 3)
        //{
        //    Debug.Log(allTerms[2][0, 0] + " " + allTerms[2][0, 1] + " " + allTerms[2][0, 2]);
        //    Debug.Log(allTerms[2][1, 0] + " " + allTerms[2][1, 1] + " " + allTerms[2][1, 2]);
        //    Debug.Log(allTerms[2][2, 0] + " " + allTerms[2][2, 1] + " " + allTerms[2][2, 2]);
        //}

        //Debug.Log("---------------------------------------------");
    }

    void InitTerms()
    {

    }

    void InitGridLayoutGroup(GridLayoutGroup grid, int size)
    {
        grid.cellSize = new Vector2((160f / size), (160f / size));
        grid.spacing = new Vector2(30f / size, 30f / size);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = size;
    }

    void Calculation()
    {
        if (allTerms.Count > 1)
        {
            for (int i = 1; i < allTerms.Count; i++)
            {
                if (i - 1 < allOperands.Count)
                {
                    switch (allOperands[i - 1])
                    {
                        case 0: CalculationPlus(i); break;
                        case 1: CalculationMinus(i); break;
                    }
                }
            }
        }

        //Debug.Log(allTerm[0][0, 0] + " " + allTerm[0][0, 1] + " " + allTerm[0][0, 2]);
        //Debug.Log(allTerm[0][1, 0] + " " + allTerm[0][1, 1] + " " + allTerm[0][1, 2]);
        //Debug.Log(allTerm[0][2, 0] + " " + allTerm[0][2, 1] + " " + allTerm[0][2, 2]);

        //Debug.Log("---------------------------------------------");

        int r = Random.Range(0, colAnswer);

        for (int i = 0; i < sizePanelTerms; i++)
        {
            for (int j = 0; j < sizePanelTerms; j++)
            {
                allAnswer[r][i, j] = allTerms[0][i, j];
                if (allTerms[0][i, j] == 1)
                {
                    allPanelAnswers[r].transform.GetChild(((i * sizePanelTerms) + j)).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    allPanelAnswers[r].transform.GetChild(((i * sizePanelTerms) + j)).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.clear;
                }
            }
        }
        //allPanelAnswers[r].GetComponent<Image>().color = Color.yellow;

        for (int k = 0; k < colAnswer; k++)
        {
            if (k != r)
            {
                for (int i = 0; i < sizePanelTerms; i++)
                {
                    for (int j = 0; j < sizePanelTerms; j++)
                    {
                        int _r = Random.Range(0, 2);
                        allAnswer[k][i, j] = _r;
                        if (_r == 1)
                        {
                            allPanelAnswers[k].transform.GetChild(((i * sizePanelTerms) + j)).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            allPanelAnswers[k].transform.GetChild(((i * sizePanelTerms) + j)).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.clear;
                        }
                    }
                }

            }
        }

        //Debug.Log(allAnswer[0][0, 0] + " " + allAnswer[1][0, 0] + " " + allAnswer[2][0, 0] + " " + allAnswer[3][0, 0]);
        //Debug.Log(allPanelAnswers[0].name + " " + allPanelAnswers[1].name + " " + allPanelAnswers[2].name + " " + allPanelAnswers[3].name);
        //Debug.Log(allPanelAnswersCubes[0][0].name + " " + allPanelAnswersCubes[1][0].name + " " + allPanelAnswersCubes[2][0].name + " " + allPanelAnswersCubes[3][0].name);
        //Debug.Log(allPanelAnswersCubes[0][0].GetComponentInChildren<Image>().color + " " + allPanelAnswersCubes[1][0].GetComponentInChildren<Image>().color + " " + allPanelAnswersCubes[2][0].GetComponentInChildren<Image>().color + " " + allPanelAnswersCubes[3][0].GetComponentInChildren<Image>().color);


        //for (int i = 0; i < sizePanelTerms; i++)
        //{
        //    for (int j = 0; j < sizePanelTerms; j++)
        //    {
        //        if (allTerm[0][i, j] == 1)
        //        {
        //            //allPanelAnswer[0].transform.GetChild(((i * sizePanelTerms) + j)).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;

        //            int n = ((i * sizePanelTerms) + j);
        //            //Debug.Log("Index: " + n.ToString());
        //            GameObject child = allPanelAnswer[0].transform.GetChild(n).gameObject;
        //            child.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.red;
        //            //Debug.Log(child.name);
        //            //allPanelAnswer[0].transform.GetChild(n).gameObject.GetComponent<Image>().color = Color.red;
        //        }
        //        else
        //        {
        //            int n = ((i * sizePanelTerms) + j);
        //            //Debug.Log("Index: " + n.ToString());
        //            GameObject child = allPanelAnswer[0].transform.GetChild(n).gameObject;
        //            child.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.clear;
        //            //Debug.Log(child.name);
        //        }
        //    }
        //}
        //for (int i = 0; i < allPanelAnswer[0].transform.childCount; i++)
        //{
        //    GameObject child = allPanelAnswer[0].transform.GetChild(i).gameObject;
        //    Debug.Log(child.name);
        //    //Do something with child
        //}
    }

    void CalculationPlus(int n)
    {
        //Debug.Log("CalculationPlus!");
        for (int i = 0; i < sizePanelTerms; i++)
        {
            for (int j = 0; j < sizePanelTerms; j++)
            {
                if (allTerms[n][i, j] == 1)
                {
                    allTerms[0][i, j] = 1;
                }
            }
        }
    }
    void CalculationMinus(int n)
    {
        //Debug.Log("CalculationMinus!");
        for (int i = 0; i < sizePanelTerms; i++)
        {
            for (int j = 0; j < sizePanelTerms; j++)
            {
                if (allTerms[n][i, j] == 1)
                {
                    allTerms[0][i, j] = 0;
                }
            }
        }
    }

    public void checkResult(int n)
    {
        for (int i = 0; i < sizePanelTerms; i++)
        {
            for (int j = 0; j < sizePanelTerms; j++)
            {
                if (allAnswer[n][i, j] != allTerms[0][i, j])
                {
                    Debug.Log("-----Wrong Answer!!!");
                    uiController.answerIndicator(false);
                    score -= wrongAnswer;
                    uiController.outScore(score, easyPoints);
                    PanelAnswers.GetComponent<Image>().color = Color.red;
                    StartCoroutine(WaitAnswer());
                    return;
                }
            }
        }
        Debug.Log("-----Current Answer!!!");
        uiController.answerIndicator(true);
        score += correctAnswer;
        uiController.outScore(score, easyPoints);
        PanelAnswers.GetComponent<Image>().color = Color.green;
        StartCoroutine(WaitAnswer());
    }

    IEnumerator WaitAnswer()
    {
        yield return new WaitForSeconds(0.3f);
        PanelAnswers.GetComponent<Image>().color = Color.clear;
        Refresh();
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
        Panel.gameObject.SetActive(gameActive);
        PanelAnswers.gameObject.SetActive(gameActive);
        PanelButtons.gameObject.SetActive(gameActive);
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
