using BestHTTP.ServerSentEvents;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum SessionState
{
    Start,
    Searching,
    OpponentNotFound,
    WaitingGameCreation,
    WaitingGameRun,
    WaitingOpponent,
    Exit, //user want to exit from session screen
    End //game created and running
}


public class SessionStateMaschine : MonoBehaviour
{
    public float opponentSearchingTime = 120f;
    private float opponentSearchingStartedAt;

    private DataController dataController;
    private SessionViewController sessionViewController;

    private SessionState state = SessionState.Start;

    //Listeners
    EventSource opponentFoundListener;
    EventSource sessionCreatedListener;
    EventSource sessionRunningListener;
    EventSource opponentReadyListener;


    private void Start()
    {
        dataController = FindObjectOfType<DataController>();
        sessionViewController = FindObjectOfType<SessionViewController>();

        StateStart();
    }
    private void Update()
    {
        if (state == SessionState.Searching && Time.time - opponentSearchingStartedAt > opponentSearchingTime && opponentSearchingStartedAt != 0)
        {
            Debug.Log("Searching time expired");
            OnOpponentNotFound();
        }
    }

    void StateStart()
    {
        Debug.Log("STATE START ()");
        
        if (dataController.GetOpponentData() != null)
        { 
            StateWaitingGameRun(true); //Он вообще вызывается?
        }
        else
        {
            StateSearching();
        }
    }

    void StateSearching()
    {
        Debug.Log("STATE SEARCHING ()");
        switch (state)
        {

        }

        
        SetState(SessionState.Searching);

        AddOnOpponentFoundListener(()=>
        {
            opponentSearchingStartedAt = Time.time;

            //edit. uncomment if game has live online sessions
            //dataController.AddUserToSearchersREST(dataController.GetUserId());
            dataController.UpdateUserStatusREST(dataController.GetUserId(), "searching");
        });

        //updating view
        sessionViewController.InitSearchingView();
    }

    void StateOpponentNotFound()
    {
        Debug.Log("STATE OPPONENT NOT FOUND ()");

        switch (state)
        {
            
        }

        opponentFoundListener.Close();
        //edit. uncomment if game has live online sessions
        //dataController.RemoveFromSearchers();

        SetState(SessionState.OpponentNotFound);

        sessionViewController.ShowOpponentNotFoundPanel();
    }

    void StateWaitingGameCreation(string gameId)
    {
        Debug.Log("STATE WAITING GAME CREATION ()");
        
        switch (state)
        {
            case SessionState.Searching:
                
                break;
        }

        sessionViewController.BlockCancelation();

        dataController.InitCurrentSession(gameId);

        opponentFoundListener.Close();

        SetState(SessionState.WaitingGameCreation);

        AddOnSessionCreatedListener(() => { Debug.Log("empty"); }); //refactor
    }

    void StateWaitingGameRun(bool alreadyRunning)
    {
        Debug.Log("STATE WAITING GAME RUN ()");

        switch (state)
        {
            case SessionState.Start:
                break;
            case SessionState.WaitingGameCreation:
                sessionCreatedListener.Close();
                break;
        }

        SetState(SessionState.WaitingGameRun);

        if (alreadyRunning) //refactor. duplicating code
        {
            //edit. add downloading session data and opponent data

            StateWaitingOpponent();
            return;
        }

        AddOnSessionRunningListener(()=>
        {
            //edit. add downloading session data and opponent data
        });
    }

    void StateExit()
    {
        Debug.Log("STATE EXIT ()");
        
        switch (state)
        {

        }

        opponentFoundListener.Close();
        //edit. uncomment if game has live online sessions
        //dataController.RemoveFromSearchers();

        SetState(SessionState.Exit);

        SceneManager.LoadScene("MenuScreen");
    }

    void StateWaitingOpponent()
    {
        Debug.Log("STATE WAITING OPPONENT ()");

        switch (state)
        {
            case SessionState.WaitingGameCreation:
                sessionCreatedListener.Close();
                break;
            case SessionState.WaitingGameRun:
                sessionRunningListener?.Close();
                break;
        }



        SetState(SessionState.WaitingOpponent);

        //edit. add realization
    }

    void StateEnd()
    {
        Debug.Log("STATE END ()");
        
        switch (state)
        {

        }

        opponentReadyListener.Close();

        SetState(SessionState.End);

        sessionViewController.ShowPreparationForTheStart(() => { SceneManager.LoadScene("Game"); });
    }

    void SetState(SessionState value)
    {
        switch (state)
        {
            case SessionState.Start:
                // state Animating exit logic
                break;
                // other states
        }
        state = value;

    }

    public void OnCanceled()
    {
        switch (state)
        {
            case SessionState.Searching:
                StateExit();
                break;
        }
    }

    public void OnOpponentNotFound()
    {
        switch (state)
        {
            case SessionState.Searching:
                StateOpponentNotFound();
                break;
        }
    }

    public void OnExit()
    {
        switch (state)
        {
            case SessionState.OpponentNotFound:
                StateExit();
                break;
        }
    }

    public void OnSearchAgain()
    {
        switch (state)
        {
            case SessionState.OpponentNotFound:
                StateSearching();
                break;
        }
    }

    

    ////////////////////////////
    private void AddOnOpponentFoundListener(Action onListenerOpened)
    {
        opponentFoundListener = FirebaseManager.Instance.Database.ListenForValueChanged($"users/searching/{dataController.GetUserId()}", (updated) =>
        {
            if (updated == "searching")
                return;

            Debug.Log("GameId was get. Id = " + updated);
            StateWaitingGameCreation(updated);
        }, (exception) =>
        {
            Debug.LogError("Exception while listening menu dashboard");
        });

        opponentFoundListener.OnOpen += (eventSource) => 
        { 
            Debug.Log($"Listener {eventSource.ConnectionKey} opened!"); 
            onListenerOpened?.Invoke(); 
        };
    }
    private void AddOnSessionCreatedListener(Action onListenerOpened)
    {
        sessionCreatedListener = FirebaseManager.Instance.Database.ListenForValueChanged($"games/{dataController.GetCurrentSessionData().Id}/status", (updated) =>
        {
            Debug.Log("OnSessionCreated(). sessionStatus = " + updated);
            if (updated == "created")
            {
                StateWaitingGameRun(false);
            }

            if (updated == "running") //Если он пропустит created
            {
                StateWaitingGameRun(true);
                //Если он пропустит состояние created
            }
        }, (exception) =>
        {
            Debug.LogError($"Error while listening session status node(checking is created). Message - {exception.Message}");
        });
    }
    private void AddOnSessionRunningListener(Action onListenerOpened)
    {
        Debug.Log("Starting listening node at path - " + $"games/{dataController.GetCurrentSessionData().Id}/status");

        sessionRunningListener = FirebaseManager.Instance.Database.ListenForValueChanged($"games/{dataController.GetCurrentSessionData().Id}/status", (updated) =>
        {
            Debug.Log("OnSessionRunning(). SessionStatus = " + updated);

            if (updated == "running")
            {
                StateWaitingOpponent();
            }
        }, (exception) =>
        {
            Debug.LogError($"Error while listening session status node(checking is running). Message - {exception.Message}");
        });

        sessionRunningListener.OnOpen += (eventSource) => { Debug.Log($"Listener {eventSource.ConnectionKey} opened!"); onListenerOpened?.Invoke(); };

        Debug.Log("[debug] Added on session running listener");
    }
    private void AddOnOpponentReadyListener(Action onListenerOpened)
    {
        Debug.Log("Starting listening node at path - " + $"games/{dataController.GetCurrentSessionData().Id}/statuses/{dataController.GetOpponentData().Id}");

        opponentReadyListener = FirebaseManager.Instance.Database.ListenForValueChanged($"games/{dataController.GetCurrentSessionData().Id}/statuses/{dataController.GetOpponentData().Id}", (updated) =>
        {
            Debug.Log("OnSessionRunning(). SessionStatus = " + updated);

            if (updated == "readyToPlay")
            {
                StateEnd();
            }
        }, (exception) =>
        {
            Debug.LogError($"Error while listening session status node(checking is running). Message - {exception.Message}");
        });

        opponentReadyListener.OnOpen += (eventSource) => { Debug.Log($"Listener {eventSource.ConnectionKey} opened!"); onListenerOpened?.Invoke(); };
        Debug.Log("[debug] Added on opponent ready listener");
    }
}
