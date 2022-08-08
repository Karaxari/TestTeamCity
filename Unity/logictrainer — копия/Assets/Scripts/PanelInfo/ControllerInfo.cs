using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public Text levelText;
    public Text formulaText;
    public Text labyrunthText;
    public Text mergeText;
    public Text uniqueDrawingText;

    DataController dataController;

    //int level = 0;


    void Start()
    {
        dataController = FindObjectOfType<DataController>();
        //level = dataController.GetUserProgressData().CurrentLevel;

        NotificationData newNotification = dataController.GetNextNotification();
        if (newNotification != null)
            onNewNotification(newNotification);

        dataController.OnMenuScreenLoaded();

        InitPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    NotificationData activeNotification;
    public void onNewNotification(NotificationData notification)
    {
        activeNotification = notification;

        //edit. add realization
    }

    void InitPanel()
    {
        string nameLevel = "level" + dataController.gameLevelConfiguration.level.ToString();
        //string nameLevel = "level1";
        Debug.Log(nameLevel);
        levelText.text = "LEVEL: " + dataController.gameLevelConfiguration.level.ToString();
        formulaText.text = dataController.GetUserProgressData().PointsDict[nameLevel].formula.ToString();
        labyrunthText.text = dataController.GetUserProgressData().PointsDict[nameLevel].labyrinth.ToString();
        mergeText.text = dataController.GetUserProgressData().PointsDict[nameLevel].merge.ToString();
        uniqueDrawingText.text = dataController.GetUserProgressData().PointsDict[nameLevel].uniqueDrawing.ToString();

        //dataController.gameLevelConfiguration.level = level;
    }

    public void leftButton()
    {
        if (dataController.gameLevelConfiguration.level < 1) return;

        dataController.gameLevelConfiguration.level--;
        InitPanel();
    }
    public void rightButton()
    {
        if (dataController.gameLevelConfiguration.level >= dataController.GetUserProgressData().CurrentLevel) return;

        dataController.gameLevelConfiguration.level++;
        InitPanel();
    }
}
