using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreenController : MonoBehaviour
{
    //public Text gamesPlayedText;
    //public Text onlinePlayersText;
    //public Text raitingValueText;
    //public Text pointsValueText;

    public GameObject userProgressViewPrefab;
    public GameObject userProgressViewWithModePrefab;
    public GameObject leaderboardScreen;
    public GameObject leaderboardContent;

    public GameObject settingsPanel;
    public GameObject settingsContent;

    DataController dataController;

    private void Start()
    {
        dataController = FindObjectOfType<DataController>();

        //UpdateDashboardView();
        //Messenger.MarkAsPermanent("OnDashboardDataUpdated");
        //Messenger.AddListener("OnDashboardDataUpdated", UpdateDashboardView);

        NotificationData newNotification = dataController.GetNextNotification();
        if(newNotification != null)
            onNewNotification(newNotification);

        dataController.OnMenuScreenLoaded();
    }
    private void OnDestroy()
    {
        //Messenger.RemoveListener("OnDashboardDataUpdated", UpdateDashboardView);
        dataController.OnMenuScreenLeft();
    }

    /* SETTINGS METHODS */
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);

        UserData currentUser = dataController.GetUserData();

        UserProfileInfoView userProfileInfoView = settingsPanel.transform.FindDeepChild("UserProfileInfoView").GetComponent<UserProfileInfoView>();
        userProfileInfoView.LoadInfo(currentUser.ProfilePhoto, currentUser.Name, currentUser.Surname, 0);
    }
    public void OnLogOutClicked()
    {
        dataController.OnLoggedOut();
        SceneManager.LoadScene("AuthScene");
    }

    /* UI HANDLERS */
    public void OnQuitClicked()
    {
        Application.Quit();
    }

    /* LEADERBOARD METHODS */
    /* Edit
     * Remove this (OnLeaderboardTabClicked and UpdateLeaderboardView with params), 
     * if your game has NO leaderboard with MODES SWITCH
     */
    bool loadingLeaderboard;
    Toggle lastChecked;
    //refactor?
    public void OnLeaderboardTabClicked(Toggle toggle)
    {
        //string gameMode = leaderboardTabGroup.GetFirstActiveToggle().name;
        if (loadingLeaderboard)
            return;

        //if (lastChecked == toggle && toggle.isOn == false)
        if (!toggle.isOn)
            return;

        Debug.LogWarning($"[Temp] Toggle - {toggle.name} value changed");

        lastChecked = toggle;

        loadingLeaderboard = true;
        //disable all toggles
        //foreach (Transform toggle in leaderboardTabGroup.transform)
        //    toggle.GetComponent<Toggle>().interactable = false;

        //Debug.Log(toggle.gameObject.name);
        UpdateLeaderboardView(toggle.gameObject.name, () =>
        {
            loadingLeaderboard = false;

            ////enable all toggles again
            //foreach (Transform toggle in leaderboardTabGroup.transform)
            //    toggle.GetComponent<Toggle>().interactable = true;
        });
    }
    private void UpdateLeaderboardView(string level, Action onUpdated = null)
    {
        ClearLeaders();

        leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(true);
        dataController.DownloadTop10((leaderboard) =>
        {
            leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(false);

            var userViews = leaderboardContent.GetComponentsInChildren<UserLeaderboardView>();
            for (int i = 0; i < 10; i++)
            {
                if (i < leaderboard.AllUsers.Length - 1)
                    userViews[i].loadViewData(i + 1, leaderboard.AllUsers[i]);
                else
                    userViews[i].gameObject.SetActive(false);
            }

            onUpdated?.Invoke();
        }, level, onUpdated);
    }

    public void OpenLeaderboard()
    {
        UpdateLeaderboardView();
        leaderboardScreen.SetActive(true);
    }
    private void UpdateLeaderboardView()
    {
        HideLeaders();

        leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(true);
        dataController.DownloadTop10((leaderboard) =>
        {
            leaderboardScreen.transform.FindDeepChild("LoadingText").gameObject.SetActive(false);
            var userViews = leaderboardContent.GetComponentsInChildren<UserLeaderboardView>(true);
            for (int i = 0; i < 10; i++)
            {
                if (i < leaderboard.AllUsers.Length - 1)
                {
                    userViews[i].loadViewData(i + 1, leaderboard.AllUsers[i]);
                    userViews[i].gameObject.SetActive(true);
                }
                else
                {
                    userViews[i].gameObject.SetActive(false);
                }
                    
            }
        });
    }
    private void HideLeaders()
    {
        foreach (Transform leaderItem in leaderboardContent.transform)
        {
            leaderItem.gameObject.SetActive(false);
        }
    }
    private void ClearLeaders()
    {
        foreach (Transform leaderItem in leaderboardContent.transform)
        {
            Destroy(leaderItem.gameObject);
        }
    }


    /* MULTIPLAYER METHODS */
    public void OnPlayWithPseudoRandom()
    {
        SceneManager.LoadScene("SessionScreen");
    }
    public void OnPlayWithFriend()
    {
        //edit. add realization

        Debug.LogWarning("OnPlatWithFriend() clicked. Empty. No Realization");
    }

    /* UPDATE VIEW METHODS */
    private void UpdateDashboardView()
    {
        //load online players info
        int onlinePlayers = dataController.GetOnlinePlayersCount() - 1; //exclusive user
        //onlinePlayersText.text = onlinePlayers.ToString();

        StatisticsData statistics = dataController.GetUserStatisticsData();
        //load game stat info
        //gamesPlayedText.text = statistics.GamesPlayed.ToString();
        
        //load score data
        UserProgressData userProgress = dataController.GetUserProgressData();
        if (userProgress != null)
        {
            //edit. add rating data
            
            //////////pointsValueText.text = userProgress.Points.ToString();
        }
        else
        {
            Debug.Log("User progress is null");
        }
    }

    /* NOTIFICATION */
    NotificationData activeNotification;
    public void onNewNotification(NotificationData notification)
    {
        activeNotification = notification;

        //edit. add realization
    }

    public void LoadScenePlayGame(int index)
    {
        SceneManager.LoadScene(index);
    }
}
