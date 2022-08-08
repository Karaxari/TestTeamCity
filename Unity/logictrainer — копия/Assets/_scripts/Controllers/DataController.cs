using BestHTTP.ServerSentEvents;
using Proyecto26;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    private string userId;
    private string idToken;

    public string menuScreenSceneName;
    public UnityEngine.UI.Slider progressBar;
    private int progress;

    UserData userData;
    UserData currentOpponent;
    LeaderboardData leaderboardData;

    SessionData currentOnlineSession;

    EventSource menuDashboardListener;

    public GameLevelConfiguration gameLevelConfiguration = new GameLevelConfiguration();

    private int onlinePlayersCount = 0;

    private Stack<NotificationData> notificationPool;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        DownloadLevelConfig(gameLevelConfiguration);
        //Отключает автоматицескую авторизацию!
        //PlayerPrefs.DeleteAll();

        userId = FirebaseManager.Instance.Auth.UserId;
        idToken = FirebaseManager.Instance.Auth.IdToken;

        notificationPool = new Stack<NotificationData>();

        //refactor. Если у игрока наберется 100 игр в профиле, то запрос будет брать много лишних данных. Может реализовать через Cloud Function?
        GetPlatformUserData(OnPlatformUserDataWasGet);

        GameAnalyticsSDK.GameAnalytics.Initialize();
    }
    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        UpdateUserStatusREST(userData.Id, "offline");
        
        //edit. uncomment if game has online sessions
        //RemoveFromSearchers();
    }

    //Platform user data
    private void GetPlatformUserData(Action<PlatformUserData> onGet)
    {
        FirebaseManager.Instance.Database.GetObject<PlatformUserData>(FirebaseProjectConfigurations.PLATFORM_DATABASE_ROOT_PATH, $"allUsers/{userId}", onGet, (exception) =>
        {
            Debug.LogError($"Error getting platform user data. Exception - {exception.Message}");
        });
    }
    private void OnPlatformUserDataWasGet(PlatformUserData pud)
    {
        if (!IsUserExistInGame(pud))
        {
            Dictionary<string, string> args = new Dictionary<string, string>() { { "game", AppConfigurations.GameName } };

            FirebaseManager.Instance.Functions.CallCloudFunctionPostObject("CreateNewUserGameData", pud, args, (statusCode) =>
            {
                Debug.Log($"User successfully created in game '{AppConfigurations.GameName}'");

                //edit. uncomment if game has online live sessions
                //RemoveFromSearchers();

                StartCoroutine(LoadAllDataREST());

            }, (exception)=>
            {
                Debug.LogError($"Error while calling CreateNewUserGameData cloud function. Message - {exception.Message}");
            });
        }
        else
        {
            StartCoroutine(LoadAllDataREST());
        }
    }
    private bool IsUserExistInGame(PlatformUserData pud)
    {
        return pud.games != null && pud.games.ContainsKey(AppConfigurations.GameName);
    }

    //Events
    public void OnMenuScreenLoaded()
    {
        InitAndStartMenuDashboardListener();
    }
    public void OnMenuScreenLeft()
    {
        if (menuDashboardListener != null)
            menuDashboardListener.Close();
    }
    public void InitAndStartMenuDashboardListener()
    {
        menuDashboardListener = FirebaseManager.Instance.Database.ListenForValueChanged($"users/{userData.Id}/public", (message) =>
        {
            Debug.Log("OnMenuDashboardDataUpdated. Updated value - " + message);
            DownloadUserDataRest(userId, (loaded) =>
            {
                userData.updatePublicData(loaded.ProgressData, loaded.Statistics);
                //Messenger.Broadcast("OnDashboardDataUpdated");
            });
        }, (exception) =>
        {
            Debug.LogError("Exception while listening menu dashboard");
        });

        gameLevelConfiguration.level = GetUserProgressData().CurrentLevel;

        //Иницируем локальную переменную уровня при первой загрузке сцены меню    //Костыль чтобы выбранный уровень игры не сбрасывался к максимальному!!!
        //if (gameLevelConfiguration.level == -1)
        //{
        //    gameLevelConfiguration.level = GetUserProgressData().CurrentLevel;
        //}
    }


    //User/Opponent data
    public UserProgressData GetUserProgressData()
    {
        return userData.ProgressData;
    }
    public StatisticsData GetUserStatisticsData()
    {
        return userData.Statistics;
    }
    public string GetUserId()
    {
        return userData.Id;
    }
    public UserData GetUserData()
    {
        return userData;
    }
    public void UpdateUserStatusREST(string user, string status)
    {
        Debug.Log($"Updting user status to: {status}");

        FirebaseManager.Instance.Database.PutValue($"users/{user}/public/status", status, () =>
        {
            Debug.Log("Success updating user status");
        }, (exception) =>
        {
            Debug.LogError("Exception while updating user status. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Critical,
                "Error while updating user status. Message - " + exception.Message);
        });
    }
    public UserData GetOpponentData()
    {
        return currentOpponent;
    }
    public void UpdateCurrentOpponent(UserData opponentData)
    {
        this.currentOpponent = opponentData;
    }
    public void UpdateOpponentData()
    {
        string opponentId = currentOnlineSession.Users.FirstOrDefault(u => u.Key != userData.Id).Key;
        currentOpponent = new UserData(opponentId);

        DownloadUserDataRest(opponentId, (loadedOpponentData) =>
        {
            currentOpponent.CopyFrom(loadedOpponentData);
        });
    }
    public void ClearOpponentData()
    {
        currentOpponent = null;
    }

    //Leaderboard data
    public LeaderboardData GetLeaderboardData()
    {
        return leaderboardData;
    }
    public int GetUserPositionInLeaderboard(string userId)
    {
        //temp. edit
        return 100;
    }
    //Delete?
    //public UserData[] SortUsersByPoints()
    //{
    //    List<UserData> usersList = new List<UserData>();
    //    usersList.AddRange(leaderboardData.AllUsers);
    //    List<UserData> sortedUsersList = usersList.OrderBy(o => o.ProgressData.Points).ToList();

    //    Debug.Log("Sorted list of users - " + Utils.CollectionUtils.ListToString(sortedUsersList));

    //    return sortedUsersList.ToArray();
    //}
    public void DownloadTop10(Action<LeaderboardData> onLoaded)
    {
        Dictionary<string, object> args = new Dictionary<string, object>() { { "game", AppConfigurations.GameName } };
        FirebaseManager.Instance.Functions.CallCloudFunction("DownloadTop10Leaderboard", args, (data) =>
        {
            DataParser.ParseLeaderboardData(data.body, out leaderboardData);
            //edit. uncomment if you sorting leaderboard on client side
            leaderboardData.SortDescending();
            onLoaded(leaderboardData);
        }, (exception)=>
        {
            Debug.LogError("Error while downloading leaderboard data");
        });
        
    }
    public void DownloadTop10(Action<LeaderboardData> onLoaded, string mode, Action onFailed = null)
    {

        Dictionary<string, object> args = new Dictionary<string, object>() { { "game", AppConfigurations.GameName }, { "mode", mode } };

        FirebaseManager.Instance.Functions.CallCloudFunction("DownloadTop10LeaderboardUpdated", args, (data) =>
        {
            DataParser.ParseLeaderboardData(data.body, out leaderboardData);
            //edit. uncomment if you sorting leaderboard on client side
            leaderboardData.SortDescending();
            onLoaded(leaderboardData);
        }, (exception) =>
        {
            Debug.LogError("Error while downloading leaderboard data");
            onFailed?.Invoke();
        });

    }

    // Session data
    public void InitCurrentSession(string sessionId)
    {
        currentOnlineSession = new SessionData(sessionId);
    }
    public void UpdateCurrentOnlineSessionData(SessionData sd)
    {
        currentOnlineSession = sd;
    }
    public SessionData GetCurrentSessionData()
    {
        return currentOnlineSession;
    }   
    public void ClearSessionData()
    {
        currentOnlineSession = null;
    }

    // Notifications
    public NotificationData GetNextNotification()
    {
        if (notificationPool.Count == 0)
            return null;

        return notificationPool.Pop();
    }
    public void OnNotificationWasShown(NotificationData notification)
    {
        RemoveNotificationREST(notification);
    }
    private void RemoveNotificationREST(NotificationData notification)
    {
        FirebaseManager.Instance.Database.Delete($"users/{userId}/private/notifications/{notification.Key}", () =>
        {
            Debug.Log("Success removing notification");
        }, (exception) =>
        {
            Debug.LogError("Exception while removing from searchers. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Critical,
                $"Error while removing notifications. UserId - {userId}. Message - {exception.Message}");
        });
    }

    //Other data
    public int GetOnlinePlayersCount()
    {
        return onlinePlayersCount;
    }
    
    





    #region REST FIREBASE 

    FirebaseCustomYield firebaseCustomYield;
    //Loading main data
    private IEnumerator LoadAllDataREST()
    {
        firebaseCustomYield = new FirebaseCustomYield();

        //Getting Leaderboard Data
        userData = new UserData(userId);
        firebaseCustomYield.onRequestStarted();
        LoadUserData();
        yield return firebaseCustomYield;
        Debug.Log("User data was loaded. User data: " + userData);
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadLeaderboardData();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        firebaseCustomYield.onRequestStarted();
        LoadOnlinePlayersCount();
        yield return firebaseCustomYield;
        UpdateProgressBar();

        //edit. add loading settings, friends data, etc. if needed

        OnAllDataLoaded();

        
    }
    public void LoadUserData(Action onLoaded = null)
    {
        FirebaseManager.Instance.Database.GetObject<UserData>($"users/{userData.Id}/public", (data) =>
        {
            userData = data;
            userData.setId(userId);

            StartCoroutine(GetSpriteFromURL(userData.ProfilePhotoUrl, (sprite) =>
            {
                userData.setProfilePhotoSprite(sprite);
            }));

            
            firebaseCustomYield.onRequestEnd();
            onLoaded?.Invoke();

        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading user data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                $"Exception while downloading user data. Message - {exception.Message}");
        });
    }
    public void LoadLeaderboardData()
    {
        FirebaseManager.Instance.Database.GetObject<LeaderboardData>($"leaderboard/allUsers", (data) =>
        {
            leaderboardData = data;

            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            Debug.LogError($"Exception while downloading leaderboard data. Message - {exception.Message}");
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading leaderboard data. Message - " + exception.Message);
        });
    }
    private void LoadOnlinePlayersCount()
    {
        FirebaseManager.Instance.Database.GetValue($"users/online/count", (value) =>
        {
            onlinePlayersCount = int.Parse(value);
            firebaseCustomYield.onRequestEnd();
        }, (exception) =>
        {
            firebaseCustomYield.onRequestEnd();
            Debug.Log("Cannot get online players count! Message - " + exception.Message);
        });
    }
    private void OnAllDataLoaded()
    {
        Debug.Log("OnAllDataLoaded");

        UpdateUserStatusREST(userData.Id, "online");

        //edit. uncomment, if game is online
        //RemoveFromSearchers(); //clearing data

        SceneManager.LoadScene(menuScreenSceneName);
    }

    //User online
    //edit. uncomment this if game has online
    //public void AddUserToSearchersREST(string userId, Action<bool> onAdded = null)
    //{
    //    string json = "{\"" + userId + "\"" + ":" + "\"searching\"}";
    //    FirebaseManager.Instance.Database.PatchJson("users/searching", json,() =>
    //    {
    //        Debug.Log("Success adding user to searchers");
    //        onAdded?.Invoke(false);
    //    }, (exception) =>
    //    {
    //        Debug.LogError("Exception while adding user to searchers. Message - " + exception.Message);
    //        GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Critical,
    //            "Error while adding user to searchers. Message - " + exception.Message);

    //        onAdded?.Invoke(true);
    //    });
    //}
    //public void RemoveFromSearchers()
    //{
    //    Debug.Log("Removing user from searchers node");

    //    FirebaseManager.Instance.Database.Delete($"users/searching/{userId}", () =>
    //    {
    //        Debug.Log("Success removing from searchers");
    //    },(exception)=>
    //    {
    //        Debug.LogError("Exception while removing from searchers. Message - " + exception.Message);
    //        GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Critical,
    //            $"Error while removing from searchers. UserId - {userId}. Message - {exception.Message}");
    //    });
    //}
    public void DownloadUserDataRest(string id, Action<UserData> onLoaded)
    {
        FirebaseManager.Instance.Database.GetObject<UserData>($"users/{id}/public", (data) =>
        {
            UserData ud = data;
            ud.setId(id);

            StartCoroutine(GetSpriteFromURL(ud.ProfilePhotoUrl, (sprite) =>
            {
                currentOpponent = ud;
                currentOpponent.updateProfilePhoto(sprite);

                onLoaded(ud);//refactor. update in next versions. Загружай картинку независимо от данных
            }));
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading user data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading user data. Message - " + exception.Message);
        });

        Debug.LogWarning("Downloading opponent data at url - " + $"/users/{id}/public.json?auth={idToken}");
    }

    public void OnLoggedOut()
    {
        UpdateUserStatusREST(userData.Id, "offline");
        //edit. uncomment if game has live online session
        //RemoveFromSearchers(); //clearing data

        FirebaseManager.Instance.Auth.LogOut();

        DestroySelf();
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Other network methods
    private IEnumerator GetSpriteFromURL(string url, Action<Sprite> callback)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning("Error downloading texture. Error - " + www.error);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Warning, "Error downloading texture. Error - " + www.error);
            callback.Invoke(null);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite downloadedSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            callback.Invoke(downloadedSprite);
        }

    }
    #endregion

    private void UpdateProgressBar()
    {
        if (progressBar == null)
            return;

        progress += UnityEngine.Random.Range(0, 100 - progress);
        progressBar.value = progress / 100f;
    }

    //#refactor. Если данный способ парсинга еще используется, вынести код для парсинга в отдельный метод в класс DataParser.
    public void DownloadLevelConfig(GameLevelConfiguration gameLevelConfiguration) //(string id, Action<UserData> onLoaded)
    {        
        FirebaseManager.Instance.Database.GetJson($"/config", (jsonData) =>  //GetJson<string>($"users/{id}/public", (data) =>
        {
            JSONNode gameLevelConfigJsonObj = JSONNode.Parse(jsonData);

            foreach (var nameGame in gameLevelConfigJsonObj)
            {
                //JSONNode levelConfigJsonObj = JSONNode.Parse(nameGame.Value);
                Dictionary<string, Dictionary<string, int>> _gameLevel = new Dictionary<string, Dictionary<string, int>>();
                foreach (var gameLevel in nameGame.Value)
                {
                    Dictionary<string, int> _gameConfig = new Dictionary<string, int>();
                    foreach (var gameConfig in gameLevel.Value)
                    {
                        _gameConfig.Add(gameConfig.Key, Int32.Parse(gameConfig.Value));
                    }
                    _gameLevel.Add(gameLevel.Key, _gameConfig);
                }
                gameLevelConfiguration.gameLevelConfig.Add(nameGame.Key, _gameLevel);
            }
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading user data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading user data. Message - " + exception.Message);
        });
    }

    public void SaveSessionToDatabase(GameSessionData session)
    {
        Dictionary<string, string> @params = new Dictionary<string, string>();
        @params.Add("userId", userData.Id);
        //@params.Add("result", game_res.ToString());
        //@params.Add("mode", game_mode.ToString());

        FirebaseManager.Instance.Functions.CallCloudFunctionPostObject<GameSessionData>("LogicGameSessionEnded", session, @params, (data) =>
        {
            Debug.Log($"Success calling function LogicGameSessionEnded");
        }, (exception) =>
        {
            Debug.LogError($"Error while calling LogicGameSessionEnded");
            //GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error, $"Exception while calling AttentionGameSessionEndeddata. Message - {exception.Message}");

        });
    }

    public void SendLevelUpDatabase()
    {
        Dictionary<string, object> @params = new Dictionary<string, object>();
        @params.Add("userId", userData.Id);
        //@params.Add("result", game_res.ToString());
        //@params.Add("mode", game_mode.ToString());

        FirebaseManager.Instance.Functions.CallCloudFunction("OnNewLevel", @params, (data) =>
        {
            Debug.Log($"Success calling function OnNewLevel");
        }, (exception) =>
        {
            Debug.LogError($"Error while calling OnNewLevel");
        });
    }

    public void CheckLevelProgress()
    {
        List<string> gameNames = new List<string>() { GameModes.formula, GameModes.labyrinth, GameModes.merge, GameModes.uniqueDrawing };
        Dictionary<string, int> gameResult = new Dictionary<string, int>();
        foreach (string name in gameNames)
        {
            var game = gameLevelConfiguration.gameLevelConfig[name];
            var levelConfig = game["level" + GetUserProgressData().CurrentLevel.ToString()];
            gameResult.Add(name, levelConfig["easyPoints"]);
        }

        string nameLevel = "level" + GetUserProgressData().CurrentLevel.ToString();

        if (GetUserProgressData().PointsDict[nameLevel].formula >= gameResult[GameModes.formula] &&
            GetUserProgressData().PointsDict[nameLevel].labyrinth >= gameResult[GameModes.labyrinth] &&
            GetUserProgressData().PointsDict[nameLevel].merge >= gameResult[GameModes.merge] &&
            GetUserProgressData().PointsDict[nameLevel].uniqueDrawing >= gameResult[GameModes.uniqueDrawing]
            )
        {
            Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%SendLevelUpDatabase%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            SendLevelUpDatabase();
        }
    }

    public void LoadingUserData()
    {
        LoadUserData(() =>
        {
            Debug.Log("Load data!");
        });
    }
}
