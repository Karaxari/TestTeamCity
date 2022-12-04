using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kadyrkaragishiev.Dialogues;
using kadyrkaragishiev.ReactiveState;
using MyBox;
using UnityEngine;
using Newtonsoft.Json;

namespace kadyrkaragishiev.Content
{
    public class SessionController : MonoBehaviour
    {
        public static SessionController Instance;
        public List<DialogueScriptableStateMachine> AllDialogues;
        public Session ActiveSession;
        public event Action SessionUpdated;
        [SerializeField] private string _path = $"{typeof(SessionController)}.json";
        private string _persistentPath => Path.Join(Application.persistentDataPath, _path);

        public List<Session> Sessions = new();

        public void InvokeSessionsUpdated() => SessionUpdated?.Invoke();

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

        private void OnEnable() => AllDialogues.ForEach(x =>
        {
            x.StateChanged += QuestStateChanged;
            x.ScoreUpdate += QuestScoreUpdate;
        });

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

            _ = ActiveSession.Save(AllDialogues);

            SessionUpdated?.Invoke();
        }

        [ContextMenu("Save")]
        public void Save() => File.WriteAllText(_persistentPath, JsonConvert.SerializeObject(Sessions.Select(x => new SessionJsonValues(x))));
        public void Save(Session active) => File.WriteAllText(_persistentPath, JsonConvert.SerializeObject(active));

        [ContextMenu("Load")]
        public void Load()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Sessions  = JsonConvert.DeserializeObject<List<SessionJsonValues>>(File.Exists(_persistentPath) ? File.ReadAllText(_persistentPath) : "[]")
                .Select(x => x.Populate(new Session (), AllDialogues))
                .ToList();
            Debug.Log("Sessions loaded" + Sessions.Count);
            SessionUpdated?.Invoke();
        }

        private Session GetSession(string pn)
        {
            foreach (var session in Sessions )
            {
                Debug.Log(session.PlayerName);
            }
            Debug.Log(Sessions.First(x=>x.PlayerName==pn));
            return Sessions.First(x=>x.PlayerName==pn);  
        } 

        public Session GetOrCreateSession(string pn,List<DialogueScriptableStateMachine> dialogues = null)
        {
            if (Sessions.Count > 0)
            {
                return GetSession("User");
            }
            Debug.Log("CREATED NEW SESSION");
            var s = new Session {PlayerName = pn,Dialogues = (dialogues ?? AllDialogues).ToDictionary(x => x, x => x.DefaultState)};
            Sessions.Add(s);
            SessionUpdated?.Invoke();
            Save();
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
            SessionUpdated?.Invoke();
        }
    }
}
