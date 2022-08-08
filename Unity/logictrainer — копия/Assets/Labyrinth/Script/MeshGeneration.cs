using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeshGeneration : MonoBehaviour
{
    public int MAXwidthMesh = 10;
    public int MAXheightMesh = 6;
    public int widthMesh = 6;
    public int heightMesh = 6;
    //public int sizePoint = 1;
    public int stepMove = 50; // количество шагов в анимации

    public int colAnswer = 5;
    public int numSprite = 4;

    public float timerGame = 60;
    public int correctAnswer = 5;
    public int wrongAnswer = 7;
    int score = 0;

    public int level = 0;

    public int easyPoints = 20;
    public int normalPoints = 35;
    public int hardPoints = 50;

    public Transform Scene;
    public Transform PanelAnswer;
    public Transform PanelUI;
    public Transform PanelExit;
    public Transform PanelEndGame;

    public GameObject PointPrefab;
    public GameObject SpritePrefab;
    public GameObject MoveAnimationObjectPrefab;

    public Sprite[] allSprite;

    //int timePause = 200;
    GameObject MoveAnimationObject;

    Point locationPoint = new Point(0, 0); // точка расположения квадратика на сетке таблицы!
    Point routeCheckPoint = new Point(0, 0); // точка проверки корректности сгенерированного маршрута по сетке таблицы!
    Point startingPoint = new Point(0, 0); // стартовая точка на сетке таблицы!

    GameObject[,] allPoint;
    List<Point> wayPoints = new List<Point>();

    List<GameObject> allSpritePoint = new List<GameObject>(); 
    List<int> waySprite = new List<int>();
    List<List<int>> allWay = new List<List<int>>()
    {
        new List<int> { 0 },
        new List<int> { 1 },
        new List<int> { 2 },
        new List<int> { 3 },
        new List<int> { 0, 1 },
        new List<int> { 0, 2 },
        new List<int> { 1, 0 },
        new List<int> { 1, 3 },
        new List<int> { 2, 0 },
        new List<int> { 2, 3 },
        new List<int> { 3, 1 },
        new List<int> { 3, 2 },

        new List<int> { 0, 1, 0 },
        new List<int> { 0, 1, 3 },
        new List<int> { 0, 2, 0 },
        new List<int> { 0, 2, 3 },

        new List<int> { 1, 0, 1 },
        new List<int> { 1, 0, 2 },
        new List<int> { 1, 3, 1 },
        new List<int> { 1, 3, 2 },

        new List<int> { 2, 0, 1 },
        new List<int> { 2, 0, 2 },
        new List<int> { 2, 3, 1 },
        new List<int> { 2, 3, 2 },

        new List<int> { 3, 1, 0 },
        new List<int> { 3, 1, 3 },
        new List<int> { 3, 2, 0 },
        new List<int> { 3, 2, 3 }
    };


    bool gameActive = false;
    bool uiActive = false;
    //int r1 = 0, r2 = 0;
    //int _r1 = 0, _r2 = 0;

    DataController dataController;
    public UIController uiController;
    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        InitLoadData();

        gameActive = true;
        uiActive = true;

        GenerationMesh();
        ActivatePoint();
        PathGeneration();
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
        var game = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.labyrinth];
        var levelConfig = game["level" + dataController.gameLevelConfiguration.level.ToString()];

        correctAnswer = levelConfig[GameConfig.correctAnswer];
        wrongAnswer = levelConfig[GameConfig.wrongAnswer];
        timerGame = levelConfig[GameConfig.time];
        colAnswer = levelConfig[GameConfig.colAnswer];
        heightMesh = levelConfig[GameConfig.heightMesh];
        widthMesh = levelConfig[GameConfig.widthMesh];

        MAXheightMesh = heightMesh;
        MAXwidthMesh = widthMesh;

        level = levelConfig[GameConfig.level];

        easyPoints = levelConfig[GameConfig.easyPoints];
        normalPoints = levelConfig[GameConfig.normalPoints];
        hardPoints = levelConfig[GameConfig.hardPoints];
        numSprite = levelConfig[GameConfig.numberSprite];

        Debug.Log("Test Load Data: " + easyPoints);
        //Debug.Log(level);
    }

    void InitResultGame()
    {
        if (score > dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].labyrinth)
        {
            GameSessionData gameSession = new GameSessionData(score, GameModes.labyrinth, level);
            dataController.SaveSessionToDatabase(gameSession);
            dataController.GetUserProgressData().PointsDict["level" + dataController.gameLevelConfiguration.level.ToString()].labyrinth = score;
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

    void GenerationMesh()
    {
        uiController.outScore(score, easyPoints);
        uiController.outTimer(timerGame);

        PanelAnswer.GetComponent<GridLayoutGroup>().constraintCount = MAXwidthMesh;
        allPoint = new GameObject[MAXheightMesh, MAXwidthMesh];
        for (int i=0; i< MAXheightMesh; i++)
        {
            for (int j = 0; j < MAXwidthMesh; j++)
            {
                GameObject point = (GameObject)Instantiate(PointPrefab, PanelAnswer.transform.position, PanelAnswer.transform.rotation);
                point.transform.parent = PanelAnswer.transform;
                int ind = (i * MAXwidthMesh) + j;
                point.name = "Point_" + ind.ToString();
                point.transform.localScale = new Vector3(1f, 1f, 1f);
                allPoint[i, j] = point;
            }
        }

        for(int i = 0; i<colAnswer; i++)
        {
            GameObject point = (GameObject)Instantiate(SpritePrefab, PanelUI.transform.position, PanelUI.transform.rotation);
            point.transform.parent = PanelUI.transform;
            point.name = "Srite_" + i.ToString();
            point.transform.localScale = new Vector3(1f, 1f, 1f);
            allSpritePoint.Add(point);
        }
        deactivatePointPanelUI();

        MoveAnimationObject = (GameObject)Instantiate(MoveAnimationObjectPrefab, PanelAnswer.transform.position, PanelAnswer.transform.rotation);
        MoveAnimationObject.transform.parent = PanelAnswer.transform;
        MoveAnimationObject.transform.localScale = new Vector3(1f, 1f, 1f);
        MoveAnimationObject.SetActive(true);
        MoveAnimationObject.GetComponent<Image>().color = Color.red;
        MoveAnimationObject.GetComponent<Image>().enabled = false;
        MoveAnimationObject.transform.parent = Scene.transform;
    }

    void deactivatePointPanelUI()
    {
        foreach(GameObject obj in allSpritePoint)
        {
            obj.SetActive(false);
        }
        for (int i=0; i<MAXheightMesh; i++)
        {
            for(int j =0; j<MAXwidthMesh; j++)
            {
                allPoint[i, j].SetActive(false);
            }
        }
    }

    public void PathGeneration()
    {
        if (gameActive)
        {
            routeCheckPoint.copyPoint(locationPoint);
            Debug.Log(routeCheckPoint.x.ToString() + ", " + routeCheckPoint.y.ToString());
            wayPoints.Add(new Point(routeCheckPoint));
            for (int i = 0; i < colAnswer;)
            {
                int r = Random.Range(0, numSprite);
                bool chek = true;
                foreach (int j in allWay[r])
                {
                    if (!chekWay(j))
                    {
                        chek = false;
                    }
                }
                routeCheckPoint.copyPoint(locationPoint);
                if (chek)
                {
                    waySprite.Add(r);
                    i++;
                    foreach (int j in allWay[r])
                    {
                        if (chekWay(j))
                        {
                            wayPoints.Add(new Point(routeCheckPoint));
                        }
                    }
                    locationPoint.copyPoint(routeCheckPoint);
                }
            }

            Debug.Log("Target Point: " + locationPoint.x.ToString() + ", " + locationPoint.y.ToString());
            //gameActive = false;
            //StartCoroutine(moveWay());
            drawWay();
        }
    }

    void drawWay()
    {
        for (int i = 0; i < waySprite.Count; i++)
        {
            allSpritePoint[i].SetActive(true);
            allSpritePoint[i].GetComponentInChildren<Image>().sprite = allSprite[waySprite[i]];
        }

        PanelAnswer.GetComponent<GridLayoutGroup>().constraintCount = widthMesh;
        for (int i = 0; i < heightMesh; i++)
        {
            for (int j = 0; j < widthMesh; j++)
            {
                allPoint[i, j].SetActive(true);
                int ind = (i * widthMesh) + j;
                allPoint[i, j].name = "Point_" + ind.ToString();
            }
        }
    }

    bool chekWay(int k)
    {
        switch (k) {
            case 0: routeCheckPoint.x++; if (routeCheckPoint.x < heightMesh) return true; else routeCheckPoint.x--; break; // Down
            case 1: routeCheckPoint.y--; if (routeCheckPoint.y > -1) return true; else routeCheckPoint.y++; break; // Left
            case 2: routeCheckPoint.y++; if (routeCheckPoint.y < widthMesh) return true; else routeCheckPoint.y--; break; // Right
            case 3: routeCheckPoint.x--; if (routeCheckPoint.x > -1) return true; else routeCheckPoint.x++; break; // Up
        }
        return false;
    }

    void refreshPoint()
    {
        wayPoints.Clear();
        gameActive = true;
        uiActive = true;
        deactivatePointPanelUI();
        waySprite.Clear();

        Debug.Log("--------------------------------------------");
        PathGeneration();
    }

    public void checkResult(int x, int y)
    {
        Debug.Log("x: " + x + ", y: " + y);
        if (uiActive)
        {
            uiActive = false;

            if (locationPoint.x == x && locationPoint.y == y)
            {
                uiController.answerIndicator(true);
                StartCoroutine(WaitAndDoCorrect());
                score += correctAnswer;
            }
            else
            {
                uiController.answerIndicator(false);
                StartCoroutine(WaitAndDoWrong());
                score -= wrongAnswer;
            }

            uiController.outScore(score, easyPoints);
        }
        startingPoint.copyPoint(locationPoint);
    }
    public void ActivatePoint()
    {
        allPoint[locationPoint.x, locationPoint.y].GetComponentInChildren<Image>().color = Color.green;
        locationPoint.x = Random.Range(0, heightMesh);
        locationPoint.y = Random.Range(0, widthMesh);
        allPoint[locationPoint.x, locationPoint.y].GetComponentInChildren<Image>().color = Color.red;
        
        startingPoint.copyPoint(locationPoint);
    }

    IEnumerator WaitAndDoCorrect()
    {
        yield return directPathMoving();
        //yield return smoothMovePath();
        //yield return movePath();
        refreshPoint();
    }
    IEnumerator WaitAndDoWrong()
    {
        yield return cancelPath();
        refreshPoint();
    }

    IEnumerator movePath()
    {
        for (int i = 1; i < wayPoints.Count; i++)
        {
            allPoint[wayPoints[i].x, wayPoints[i].y].GetComponentInChildren<Image>().color = Color.red;
            allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].GetComponentInChildren<Image>().color = Color.yellow;
            yield return new WaitForSeconds(0.05f);
            allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].GetComponentInChildren<Image>().color = Color.green;
        }
    }

    IEnumerator directPathMoving()
    {
        Point p1 = startingPoint;
        Point p2 = wayPoints[wayPoints.Count - 1];

        allPoint[wayPoints[0].x, wayPoints[0].y].GetComponentInChildren<Image>().color = Color.green;
        MoveAnimationObject.GetComponent<Image>().enabled = true;
        MoveAnimationObject.transform.position = new Vector3(allPoint[p1.x, p1.y].transform.position.x, allPoint[p1.x, p1.y].transform.position.y, allPoint[p1.x, p1.y].transform.position.z);


        float cof_x = allPoint[p2.x, p2.y].transform.position.x - allPoint[p1.x, p1.y].transform.position.x;
        float cof_y = allPoint[p2.x, p2.y].transform.position.y - allPoint[p1.x, p1.y].transform.position.y;

        cof_x = cof_x / stepMove;
        cof_y = cof_y / stepMove;

        for (int i=0; i< stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveAnimationObject.transform.Translate(cof_x, cof_y, 0);
        }

        MoveAnimationObject.transform.position = new Vector3(allPoint[p2.x, p2.y].transform.position.x, allPoint[p2.x, p2.y].transform.position.y, allPoint[p2.x, p2.y].transform.position.z);
        MoveAnimationObject.GetComponent<Image>().enabled = false;
        allPoint[p2.x, p2.y].GetComponentInChildren<Image>().color = Color.red;
        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator smoothMovePath()
    {
        allPoint[wayPoints[0].x, wayPoints[0].y].GetComponentInChildren<Image>().color = Color.green;
        MoveAnimationObject.GetComponent<Image>().enabled = true;
        MoveAnimationObject.transform.position = new Vector3(allPoint[startingPoint.x, startingPoint.y].transform.position.x, allPoint[startingPoint.x, startingPoint.y].transform.position.y, allPoint[startingPoint.x, startingPoint.y].transform.position.z);

        switch (directionСheck(startingPoint, wayPoints[0]))
        {
            case 0: yield return StartCoroutine(smoothMovePathDown(startingPoint, wayPoints[0])); break;
            case 1: yield return StartCoroutine(smoothMovePathLeft(startingPoint, wayPoints[0])); break;
            case 2: yield return StartCoroutine(smoothMovePathRight(startingPoint, wayPoints[0])); break;
            case 3: yield return StartCoroutine(smoothMovePathUp(startingPoint, wayPoints[0])); break;
            default: Debug.Log("Error MovePath"); break;
        }
        yield return new WaitForSeconds(0.1f);

        for (int i = 1; i < wayPoints.Count; i++)
        {
            Debug.Log("x: " + allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.x + ", y: " + allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.y + ", z: " + allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.z);
            MoveAnimationObject.transform.position = new Vector3(allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.x, allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.y, allPoint[wayPoints[i - 1].x, wayPoints[i - 1].y].transform.position.z);
            switch (directionСheck(wayPoints[i - 1], wayPoints[i]))
            {
                case 0: yield return StartCoroutine(smoothMovePathDown(wayPoints[i - 1], wayPoints[i])); break;
                case 1: yield return StartCoroutine(smoothMovePathLeft(wayPoints[i - 1], wayPoints[i])); break;
                case 2: yield return StartCoroutine(smoothMovePathRight(wayPoints[i - 1], wayPoints[i])); break;
                case 3: yield return StartCoroutine(smoothMovePathUp(wayPoints[i - 1], wayPoints[i])); break;
                default: Debug.Log("Error MovePath"); break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        MoveAnimationObject.transform.position = new Vector3(allPoint[wayPoints[wayPoints.Count - 1].x, wayPoints[wayPoints.Count - 1].y].transform.position.x, allPoint[wayPoints[wayPoints.Count - 1].x, wayPoints[wayPoints.Count - 1].y].transform.position.y, allPoint[wayPoints[wayPoints.Count - 1].x, wayPoints[wayPoints.Count - 1].y].transform.position.z);
        MoveAnimationObject.GetComponent<Image>().enabled = false;
        allPoint[wayPoints[wayPoints.Count - 1].x, wayPoints[wayPoints.Count - 1].y].GetComponentInChildren<Image>().color = Color.red;
        yield return new WaitForSeconds(0.05f);
    }
    IEnumerator smoothMovePathDown(Point p1, Point p2)
    {
        float res = allPoint[p1.x, p1.y].transform.position.y - allPoint[p2.x, p2.y].transform.position.y;
        float cof = res / stepMove;
        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveAnimationObject.transform.Translate(0, -cof, 0);
        }
    }
    IEnumerator smoothMovePathLeft(Point p1, Point p2)
    {
        float res = allPoint[p1.x, p1.y].transform.position.x - allPoint[p2.x, p2.y].transform.position.x;
        float cof = res / stepMove;
        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveAnimationObject.transform.Translate(-cof, 0, 0);
        }
    }

    IEnumerator smoothMovePathRight(Point p1, Point p2)
    {
        float res = allPoint[p2.x, p2.y].transform.position.x - allPoint[p1.x, p1.y].transform.position.x;
        float cof = res / stepMove;
        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveAnimationObject.transform.Translate(cof, 0, 0);
        }
    }

    IEnumerator smoothMovePathUp(Point p1, Point p2)
    {
        float res = allPoint[p2.x, p2.y].transform.position.y - allPoint[p1.x, p1.y].transform.position.y;
        float cof = res / stepMove;
        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            MoveAnimationObject.transform.Translate(0, cof, 0);
        }
    }
    IEnumerator cancelPath()
    {
        //allPoint[wayPoints[0].x, wayPoints[0].y].GetComponentInChildren<Image>().color = Color.green;
        //ActivatePoint();
        //yield return new WaitForSeconds(0.10f);

        allPoint[locationPoint.x, locationPoint.y].GetComponentInChildren<Image>().color = Color.blue;
        yield return new WaitForSeconds(0.5f);
        allPoint[wayPoints[0].x, wayPoints[0].y].GetComponentInChildren<Image>().color = Color.green;
        allPoint[locationPoint.x, locationPoint.y].GetComponentInChildren<Image>().color = Color.red;
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
        PanelAnswer.gameObject.SetActive(gameActive);
        PanelUI.gameObject.SetActive(gameActive);
    }
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    int directionСheck(Point p1, Point p2)
    {
        if (p1.x < p2.x && p1.y == p2.y) return 0; //Down
        if (p1.x == p2.x && p1.y > p2.y) return 1; //Left
        if (p1.x == p2.x && p1.y < p2.y) return 2; //Right
        if (p1.x > p2.x && p1.y == p2.y) return 3; //Up
        return -1;
    }
}
