using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawingController : MonoBehaviour
{
    public int MAXWidthPanel = 8; //Максимальная ширина сетки
    public int MAXHeightPanel = 4; //Максимальная высота сетки
    public int widthPanel = 3;
    public int heightPanel = 3;
    public int numberFigure = 9; //Количество Фигур
    public int numberForma = 9; //Количество Форм
    public int numberColor = 9; //Количество Цветов

    public int _number_figure = 6; //Ограничитель Количества Фигур
    public int _number_forma = 6; //Ограничитель Количества Форм
    public int _number_color = 6; //Ограничитель Количества Цветов

    public int difficultyLevel = 1; //Уровень сложности критериев выборки 

    public float timerGame = 60;
    public int correctAnswer = 5;
    public int wrongAnswer = 7;
    public int diversitySprite = 5;
    int score = 0;

    public int level = 0;

    public int easyPoints = 20;
    public int normalPoints = 35;
    public int hardPoints = 50;

    public Transform PanelButtons;
    public Transform PanelExit;
    public Transform PanelEndGame;
    public GameObject PrefabButton;
    public GameObject Indicator;

    public Sprite[] allSprite;

    List<GameObject> allButons = new List<GameObject>();

    //List<int> indSameFigure = new List<int>();
    //List<int> indSameForm = new List<int>();
    //List<int> indSameColor = new List<int>();
    //List<int> indSumm;
    List<int> arrIndex = new List<int>();

    //Dictionary<int, List<int>> indUniqueFigure = new Dictionary<int, List<int>>();
    //Dictionary<int, List<int>> indUniqueForm = new Dictionary<int, List<int>>();
    //Dictionary<int, List<int>> indUniqueColor = new Dictionary<int, List<int>>();

    Dictionary<int, List<int>> gridLevel = new Dictionary<int, List<int>>();
   
    CollectionNameImage indImage = new CollectionNameImage();

    int Answer = 0;
    bool gameActive = false;

    DataController dataController;
    public UIController uiController;
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        InitLoadData();

        Indicator.GetComponentInChildren<Image>().color = Color.clear;
        GenerationPanel();
        initGridLevel();
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
        var game = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.uniqueDrawing];
        var levelConfig = game["level" + dataController.gameLevelConfiguration.level.ToString()];

        correctAnswer = levelConfig[GameConfig.correctAnswer];
        wrongAnswer = levelConfig[GameConfig.wrongAnswer];
        timerGame = levelConfig[GameConfig.time];
        diversitySprite = levelConfig[GameConfig.diversitySprite];
        heightPanel = levelConfig[GameConfig.heightMesh];
        widthPanel = levelConfig[GameConfig.widthMesh];
        difficultyLevel = levelConfig[GameConfig.difficultyLevel];

        MAXHeightPanel = heightPanel;
        MAXWidthPanel = widthPanel;

        level = levelConfig[GameConfig.level];

        easyPoints = levelConfig[GameConfig.easyPoints];
        normalPoints = levelConfig[GameConfig.normalPoints];
        hardPoints = levelConfig[GameConfig.hardPoints];

        Debug.Log("Test Load Data: " + easyPoints);
        //Debug.Log(level);
    }

    void InitResultGame()
    {
        if (score > dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].uniqueDrawing)
        {
            GameSessionData gameSession = new GameSessionData(score, GameModes.uniqueDrawing, level);
            dataController.SaveSessionToDatabase(gameSession);
            dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].uniqueDrawing = score;
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

        //initIndexSprite();
        initIndexSprite_2();

        //initArrayAnswer();
        InitPanel();
    }

    void initGridLevel()
    {
        gridLevel.Add(9, new List<int>() { 3, 3, 2, 1 });
        gridLevel.Add(12, new List<int>() { 4, 4, 3, 1 });
        gridLevel.Add(15, new List<int>() { 5, 4, 3, 2, 1 });
        gridLevel.Add(16, new List<int>() { 5, 4, 3, 3, 1 });
        gridLevel.Add(18, new List<int>() { 6, 4, 4, 3, 1 });
        gridLevel.Add(20, new List<int>() { 7, 5, 4, 3, 1 });
        gridLevel.Add(24, new List<int>() { 8, 6, 4, 3, 2, 1 });
        gridLevel.Add(28, new List<int>() { 10, 7, 5, 3, 2, 1 });
        gridLevel.Add(32, new List<int>() { 11, 8, 6, 4, 2, 1 });
    }

    void initIndexSprite_2()
    {
        int size = (numberForma * numberColor);

        int unique_index = Random.Range(0, _number_forma);
        int ind_2 = Random.Range(0, _number_figure);
        int ind_3 = Random.Range(0, _number_color);

        List<int> arr = gridLevel[widthPanel * heightPanel];

        foreach (int siz in arr)
        {
            if (siz == 1)
            {
                switch (difficultyLevel)
                {
                    case 1: Answer = (ind_2 * size) + (ind_3 * numberColor) + unique_index; break;
                    case 2: Answer = (unique_index * size) + (ind_2 * numberFigure) + ind_3; break;
                    case 3: Answer = (ind_2 * size) + (unique_index * numberForma) + ind_3; break;
                }
                arrIndex.Add(Answer);
                break;
            }
            int _i1 = Random.Range(0, _number_forma);
            while (_i1 == unique_index)
            {
                _i1 = Random.Range(0, _number_forma);
            }
            for (int i = 0; i < siz; i++)
            {
                int index = 0;
                switch (difficultyLevel)
                {
                    case 1: index = (ind_2 * size) + (ind_3 * numberColor) + _i1; break;
                    case 2: index = (_i1 * size) + (ind_2 * numberFigure) + ind_3; break;
                    case 3: index = (ind_2 * size) + (_i1 * numberForma) + ind_3; break;
                }

                arrIndex.Add(index);
            }
        }
    }

    void uniqueForm()
    {
        

    }
    void uniqueFigur()
    {

    }

    void uniqueColor()
    {

    }

    //void initIndexSprite()
    //{
    //    //int n = Random.Range(1, allSprite.Length + 1) - 1;
    //    int siz = (numberForma * numberColor);
    //    int _siz = (_number_forma * _number_color);

    //    //int n_figur = n / siz;
    //    //int n_form = (n % siz) / numberColor;
    //    //int n_color = (n % siz) % numberColor;

    //    int n_figur = Random.Range(0, _number_figure);
    //    int n_form = Random.Range(0, _number_forma);
    //    int n_color = Random.Range(0, _number_color);

    //    //Debug.Log("Num: " + n.ToString());
    //    //Debug.Log("№ figur: " + n_figur.ToString() + ", № Form: " + n_form.ToString() + ", № Color: " + n_color.ToString());

    //    ///--------------------------------------------------------------------------- indUniqueFigure
    //    for (int i = 0; i < _number_figure; i++)
    //    {
    //        if (i == n_figur) continue;
    //        int _i = i * siz;
    //        List<int> arr = new List<int>();
    //        for (int j = 0; j < _number_forma; j++)
    //        {
    //            int _j = j * numberForma;
    //            for (int k = 0; k < _number_color; k++)
    //            {
    //                arr.Add(_i + _j + k);
    //            }
    //        }
    //        indUniqueFigure.Add(i, arr);
    //    }
    //    n_figur *= siz;
    //    for (int i = 0; i < _number_forma; i++)
    //    {
    //        int _i = i * numberForma;
    //        for (int j = 0; j < _number_color; j++)
    //        {
    //            indSameFigure.Add(n_figur + _i + j);
    //        }
    //    }
    //    ///--------------------------------------------------------------------------- indUniqueForm
    //    for (int k = 0; k < _number_forma; k++)
    //    {
    //        if (k == n_form) continue;
    //        int _k = k * numberForma;

    //        List<int> arr = new List<int>();
    //        for (int i = 0; i < _number_figure; i++)
    //        {
    //            int _n = i * siz;
    //            for (int j = 0; j < _number_color; j++)
    //            {
    //                arr.Add((_n + _k + j));
    //            }
    //        }
    //        indUniqueForm.Add(k, arr);
    //    }
    //    n_form *= numberForma;
    //    for (int i = 0; i < _number_figure; i++)
    //    {
    //        int _n = i * siz;
    //        for (int j = 0; j < _number_color; j++)
    //        {
    //            indSameForm.Add((_n + n_form + j));
    //        }
    //    }
    //    ///--------------------------------------------------------------------------- indUniqueColor
    //    for (int j = 0; j < _number_color; j++)
    //    {
    //        if (j == n_color) continue;
    //        List<int> arr = new List<int>();
    //        for (int k = 0; k < _number_figure; k++)
    //        {
    //            int _n = k * siz;
    //            //int _k = k * numberColor;
                
    //            for (int i = 0; i < _number_forma; i++)
    //            {
    //                int _i = i * numberColor;

    //                arr.Add((_n + _i + j));
    //            }
    //        }
    //        indUniqueColor.Add(j, arr);
    //    }
    //    //n_color *= (numberForma * numberColor);
    //    for (int i = 0; i < _number_figure; i++)
    //    {
    //        int _n = i * siz;
    //        for (int j = 0; j < _number_forma; j++)
    //        {
    //            indSameColor.Add((_n + (j * numberColor) + n_color));
    //        }
    //    }
    //    ///---------------------------------------------------------------------------

    //    string str1 = "";
    //    foreach (int k in indSameFigure)
    //    {
    //        str1 += k.ToString() + ", ";
    //    }

    //    string str2 = "";
    //    foreach (int k in indSameForm)
    //    {
    //        str2 += k.ToString() + ", ";
    //    }

    //    string str3 = "";
    //    foreach (int k in indSameColor)
    //    {
    //        str3 += k.ToString() + ", ";
    //    }
    //    Debug.Log(str1);
    //    Debug.Log(str2);
    //    Debug.Log(str3);

    //    Debug.Log("$$$$$$$$$$$$$$$$$$$$$$");

    //    string str4 = "";
    //    foreach (var lis in indUniqueColor)
    //    {
    //        foreach (int k in lis.Value)
    //        {
    //            str4 += k.ToString() + ", ";
    //        }
    //        Debug.Log(lis.Key + ": " + str4);
    //    }
    //    Debug.Log(str4);

    //    Debug.Log("$$$$$$$$$$$$$$$$$$$$$$");
    //}
    //void initArrayAnswer()
    //{
    //    switch (difficultyLevel)
    //    {
    //        case 1: indSumm = new List<int>(indSameFigure.Count); indSumm.AddRange(indSameFigure); break;
    //        case 2: indSumm = new List<int>(indSameForm.Count); indSumm.AddRange(indSameForm); break;
    //        case 3: indSumm = new List<int>(indSameColor.Count); indSumm.AddRange(indSameColor); break;
    //        default:
    //            indSumm = new List<int>(indSameFigure.Count + indSameForm.Count + indSameColor.Count);
    //            indSumm.AddRange(indSameFigure);
    //            indSumm.AddRange(indSameForm);
    //            indSumm.AddRange(indSameColor); break;
    //    }

    //    List<int> arr = gridLevel[widthPanel * heightPanel];
    //    string str = "";
    //    foreach (int i in arr)
    //    {
    //        str += i.ToString() + ", ";
    //    }
    //    Debug.Log(str);

    //    foreach (int ar in arr)
    //    {
    //        if (ar == 1)
    //        {
    //            int r = Random.Range(0, indSumm.Count);
    //            arrIndex.Add(indSumm[r]);
    //            Answer = indSumm[r];
    //            //Debug.Log("Index: " + indSumm[r].ToString());
    //            break; 
    //        }

    //        switch (difficultyLevel)
    //        {
    //            case 1:
    //                int _i1 = Random.Range(0, indUniqueFigure.Count);
    //                while (!indUniqueFigure.ContainsKey(_i1))
    //                {
    //                    _i1 = Random.Range(0, indUniqueFigure.Count);
    //                }
    //                List<int> _arr1 = indUniqueFigure[_i1];
    //                for (int i = 0; i < ar; i++)
    //                {
    //                    int r = Random.Range(0, _arr1.Count);
    //                    arrIndex.Add(_arr1[r]);
    //                    _arr1.RemoveAt(r);
    //                }
    //                indUniqueFigure.Remove(_i1);
    //                break;
    //            case 2:
    //                int _i2 = Random.Range(0, indUniqueForm.Count);
    //                while (!indUniqueForm.ContainsKey(_i2))
    //                {
    //                    _i2 = Random.Range(0, indUniqueForm.Count);
    //                }
    //                List<int> _arr2 = indUniqueForm[_i2];
    //                for (int i = 0; i < ar; i++)
    //                {
    //                    int r = Random.Range(0, _arr2.Count);
    //                    arrIndex.Add(_arr2[r]);
    //                    _arr2.RemoveAt(r);
    //                }
    //                indUniqueForm.Remove(_i2);
    //                break;
    //            case 3:
    //                int _i3 = Random.Range(0, indUniqueColor.Count);
    //                while (!indUniqueColor.ContainsKey(_i3))
    //                {
    //                    _i3 = Random.Range(0, indUniqueColor.Count);
    //                }
    //                List<int> _arr3 = indUniqueColor[_i3];
    //                for (int i = 0; i < ar; i++)
    //                {
    //                    int r = Random.Range(0, _arr3.Count);
    //                    arrIndex.Add(_arr3[r]);
    //                    _arr3.RemoveAt(r);
    //                }
    //                indUniqueColor.Remove(_i3);
    //                break;
    //            default:
    //                Debug.LogError("----------------------42-------------------");
    //                return;
    //        }
    //    }

    //    str = "";
    //    foreach (int i in arrIndex)
    //    {
    //        str += i.ToString() + ", ";
    //    }
    //    Debug.Log(str);
    //    Debug.Log(indImage.name.FirstOrDefault(x => x.Value == arrIndex.Last()).Key);

    //    indSameFigure.Clear();
    //    indSameForm.Clear();
    //    indSameColor.Clear();
    //    indSumm.Clear();

    //    indUniqueFigure.Clear();
    //    indUniqueColor.Clear();
    //    indUniqueForm.Clear();
    //}

    void GenerationPanel()
    {
        uiController.outScore(score, easyPoints);
        uiController.outTimer(timerGame);

        PanelButtons.GetComponent<GridLayoutGroup>().constraintCount = MAXWidthPanel;

        for (int i = 0; i < (MAXWidthPanel * MAXHeightPanel); i++)
        {
            GameObject button = (GameObject)Instantiate(PrefabButton, PanelButtons.transform.position, PanelButtons.transform.rotation);
            button.transform.parent = PanelButtons.transform;
            button.name = "Button_" + i.ToString();
            button.SetActive(false);
            button.transform.localScale = new Vector3(1f, 1f, 1f);
            allButons.Add(button);
        }
    }

    void InitPanel()
    {
        PanelButtons.GetComponent<GridLayoutGroup>().constraintCount = widthPanel;

        for (int i = 0; i < (widthPanel * heightPanel); i++)
        {
            allButons[i].SetActive(true);
            int r = Random.Range(0, arrIndex.Count);
            allButons[i].GetComponentInChildren<Image>().sprite = allSprite[arrIndex[r]];
            arrIndex.RemoveAt(r);
        }
        arrIndex.Clear();
        for (int i = (widthPanel * heightPanel); i < (MAXWidthPanel * MAXHeightPanel); i++)
        {
            allButons[i].SetActive(false);
        }
    }
    public void checkResult(string name)
    {
        Debug.Log(name);
        Debug.Log(indImage.name[name].ToString());
        if (indImage.name[name] == Answer)
        {
            Debug.Log("Correct!");
            uiController.answerIndicator(true);
            Indicator.GetComponentInChildren<Image>().color = Color.green;
            score += correctAnswer;
        }
        else
        {
            Debug.Log("Wrong!");
            uiController.answerIndicator(false);
            Indicator.GetComponentInChildren<Image>().color = Color.red;
            score -= wrongAnswer;
        }
        uiController.outScore(score, easyPoints);
        StartCoroutine(WaitAnswer());
    }

    IEnumerator WaitAnswer()
    {
        PanelButtons.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        Indicator.GetComponentInChildren<Image>().color = Color.clear;
        Refresh();
        yield return new WaitForSeconds(0.2f);
        PanelButtons.gameObject.SetActive(true);
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
        PanelButtons.gameObject.SetActive(gameActive);
    }
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
