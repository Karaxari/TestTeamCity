using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public Text levelText;

    public Text formulaText;
    public Text labyrunthText;
    public Text mergeText;
    public Text uniqueDrawingText;
    public Text settingsText;

    public Image formulaSlider;
    public Image labyrunthSlider;
    public Image mergeSlider;
    public Image uniqueDrawingSlider;

    public Transform levelGroup;
    public Transform ratingGroup;

    public Sprite levelStock;
    public Sprite levelCurrent;
    public Sprite levelLock;

    DataController dataController;
    LeaderboardData leaderboardData;
    int maxLevel = 0;

    void Start()
    {
        dataController = FindObjectOfType<DataController>();

        //dataController.LoadUserData(() =>
        //{
        //    maxLevel = dataController.GetUserData().ProgressData.CurrentLevel;
        //    InitPanel(maxLevel);
        //});

        maxLevel = dataController.GetUserData().ProgressData.CurrentLevel;
        InitPanel(maxLevel);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitPanel(int level)
    {
        string nameLevel = "level" + level.ToString();
        //string nameLevel = "level1";
        Debug.Log(nameLevel);
        levelText.text = level.ToString();

        formulaText.text = dataController.GetUserProgressData().PointsDict[nameLevel].formula.ToString();
        labyrunthText.text = dataController.GetUserProgressData().PointsDict[nameLevel].labyrinth.ToString();
        mergeText.text = dataController.GetUserProgressData().PointsDict[nameLevel].merge.ToString();
        uniqueDrawingText.text = dataController.GetUserProgressData().PointsDict[nameLevel].uniqueDrawing.ToString();

        int formulaLimit = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.formula]["level" + level.ToString()][GameConfig.easyPoints];
        int labyrunthLimit = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.labyrinth]["level" + level.ToString()][GameConfig.easyPoints];
        int mergeLimit = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.merge]["level" + level.ToString()][GameConfig.easyPoints];
        int uniqueDrawingLimit = dataController.gameLevelConfiguration.gameLevelConfig[GameModes.uniqueDrawing]["level" + level.ToString()][GameConfig.easyPoints];

        formulaSlider.fillAmount = (float)dataController.GetUserProgressData().PointsDict[nameLevel].formula / (float)formulaLimit;
        labyrunthSlider.fillAmount = (float)dataController.GetUserProgressData().PointsDict[nameLevel].labyrinth / (float)labyrunthLimit;
        mergeSlider.fillAmount = (float)dataController.GetUserProgressData().PointsDict[nameLevel].merge / (float)mergeLimit;
        uniqueDrawingSlider.fillAmount = (float)dataController.GetUserProgressData().PointsDict[nameLevel].uniqueDrawing / (float)uniqueDrawingLimit;
    

        for (int i=0; i<levelGroup.childCount; i++)
        {
            levelGroup.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
            if (i <= maxLevel)
            {
                //levelGroup.GetChild(i).GetComponent<Image>().sprite = levelStock;
                levelGroup.GetChild(i).GetChild(0).GetComponent<Image>().sprite = levelStock;
                levelGroup.GetChild(i).GetComponentInChildren<Text>().text = i.ToString();
                Debug.Log(levelGroup.GetChild(i).name);
            }
            else
            {
                levelGroup.GetChild(i).GetChild(0).GetComponent<Image>().sprite = levelLock;
                levelGroup.GetChild(i).GetComponentInChildren<Text>().text = "";
                levelGroup.GetChild(i).GetComponent<Button>().interactable = false;
            }

        }

        levelGroup.GetChild(level).GetChild(0).GetComponent<Image>().sprite = levelCurrent;
        levelGroup.GetChild(level).GetChild(0).GetComponent<Image>().color = Color.blue;
    }

    public void choiceLevel(int level)
    {
        InitPanel(level);
        dataController.gameLevelConfiguration.level = level;
    }

    public void outSettingsInfo()
    {
        settingsText.text = dataController.GetUserData().Name;
    }

    //public void outPlayerRating()
    //{
    //    List<LeaderBoard> scoreLider = new List<LeaderBoard>();

    //    for (int i = 0; i < leaderboardData.AllUsers.Length; i++)
    //    {
    //        scoreLider.Add(new LeaderBoard(leaderboardData.AllUsers[i].ProgressData.TotalPoints, leaderboardData.AllUsers[i].Name));
    //    }

    //    sortLeaderBoard(scoreLider);

    //    Debug.Log(leaderboardData);
    //    for (int i = 0; i < ratingGroup.childCount; i++)
    //    {
    //        if (i < scoreLider.Count)
    //        {
    //            Debug.Log(ratingGroup.GetChild(i).gameObject.name);
    //            ratingGroup.GetChild(i).gameObject.transform.Find("TextName").GetComponent<Text>().text = scoreLider[i].Name;
    //            ratingGroup.GetChild(i).gameObject.transform.Find("TextScore").GetComponent<Text>().text = scoreLider[i].Score.ToString();
    //        }
    //        else
    //        {
    //            ratingGroup.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }
    //}

    //void sortLeaderBoard(List<LeaderBoard> scoreLider)
    //{
    //    for(int i=0; i< scoreLider.Count; i++)
    //    {
    //        for(int j=0; j<scoreLider.Count - 1; j++)
    //        {
    //            if (scoreLider[j].Score < scoreLider[j + 1].Score)
    //            {
    //                LeaderBoard tmp = scoreLider[j];
    //                scoreLider[j] = scoreLider[j + 1];
    //                scoreLider[j + 1] = tmp;
    //            }
    //        }
    //    }
    //}
}
