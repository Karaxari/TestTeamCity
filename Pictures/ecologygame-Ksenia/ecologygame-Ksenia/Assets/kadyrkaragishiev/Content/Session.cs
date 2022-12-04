using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using kadyrkaragishiev.Dialogues;
using UnityEngine;

namespace kadyrkaragishiev.Content
{
    public class Session
    {
        public string PlayerName;
        public Dictionary<DialogueScriptableStateMachine, DialogueState> Dialogues = new();
        public int EcologicPercent;
        public int WellFarePercent;

        public Session Activate()
        {
            _ = (SessionController.Instance.ActiveSession?.Deactivate());
            SessionController.Instance.ActiveSession = this;
            SessionController.Instance.InvokeSessionsUpdated();
            return Load();
        }

        private Session Load()
        {
            Debug.Log("Called load to change values");
            foreach ((DialogueScriptableStateMachine d, DialogueState s) in Dialogues) d.LoadState(s);
            return this;
        }
        
        public Session Deactivate()
        {
            SessionController.Instance.ActiveSession = null;
            SessionController.Instance.InvokeSessionsUpdated();
            return Save();
        }
        
        public Session Save() => Save(SessionController.Instance.AllDialogues);
        public Session Save(IEnumerable<DialogueScriptableStateMachine> dialogues)
        {
            Debug.Log("Called save");
            Dialogues = dialogues.ToDictionary(x => x, x => x.State);
            SessionController.Instance.Save();
            return this;
        }
        public Session Save(DialogueScriptableStateMachine dialogue)
        {
            Debug.Log("Called save");
            SessionController.Instance.Save(this);
            return this;
        }


        public void OnQuestScoreUpdate(DialogueScriptableStateMachine quest, int ecologicDelta, int wellFareDelta)
        {
            if (!Dialogues.ContainsKey(quest)) return;
            EcologicPercent += ecologicDelta;
            WellFarePercent += wellFareDelta;
        }
    }
}
