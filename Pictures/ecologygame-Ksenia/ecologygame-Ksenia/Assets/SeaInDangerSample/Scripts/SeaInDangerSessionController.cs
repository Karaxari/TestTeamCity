using kadyrkaragishiev.Content;
using kadyrkaragishiev.Dialogues;
using kadyrkaragishiev.ReactiveState;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SeaInDangerSessionController : SessionController
{
    public DialogueScriptableStateMachine dialogue;
    [SerializeField] private string _path = $"{typeof(SeaInDangerSessionController)}.json";
    private string _persistentPath => Path.Join(Application.persistentDataPath, _path);

#if UNITY_EDITOR
    [SerializeField] private bool _load;
#endif
    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

#if UNITY_EDITOR
        if (!_load) return;
#endif

        Load();

    }

    private IEnumerator LoadCurrentSession()
    {
        yield return new WaitForSeconds(1f);
        GetOrCreateSession("User").Activate();
        Debug.Log("User activated");
    }

    private void OnEnable()
    {
        dialogue.StateChanged += QuestStateChanged;
        dialogue.ScoreUpdate += QuestScoreUpdate;
     }

    private void OnDestroy()
    {
        foreach (var dialogue in ActiveSession.Dialogues)
        {
            Debug.Log(dialogue.Value);
        }
        Save();
    }

    private void QuestScoreUpdate(DialogueScriptableStateMachine quest, int ecologicDelta, int wellFareDelta)
    {
        if (ActiveSession == null) return;

        ActiveSession.OnQuestScoreUpdate(quest, ecologicDelta, wellFareDelta);

        _ = ActiveSession.Save(dialogue);
    }

    private Session GetSession(string pn)
    {
        foreach (var session in Sessions)
        {
            Debug.Log(session.PlayerName);
        }
        Debug.Log(Sessions.First(x => x.PlayerName == pn));
        return Sessions.First(x => x.PlayerName == pn);
    }

    public Session GetOrCreateSession(string pn, List<DialogueScriptableStateMachine> dialogues = null)
    {
        if (Sessions.Count > 0)
        {
            return GetSession("SeaInDanger");
        }
        Debug.Log("CREATED NEW SESSION Sea In Danger");
        var s = new Session { PlayerName = pn, Dialogues = (dialogues ?? AllDialogues).ToDictionary(x => x, x => x.DefaultState) };
        Save(s);
        return s;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) ActiveSession.EcologicPercent += 10;

        if (Input.GetKeyDown(KeyCode.A)) Debug.Log(ActiveSession.EcologicPercent);
    }

    [ContextMenu("RemoveSavedData")]
    private void RemoveSavedData() =>
        File.WriteAllText(_persistentPath, "[]");

    private void QuestStateChanged(IStateEmitter<DialogueState> caller, DialogueState state)
    {
        Debug.Log("Called QuestStateChanged");
        if (ActiveSession == null) return;
        _ = ActiveSession.Save(AllDialogues);
    }

}
