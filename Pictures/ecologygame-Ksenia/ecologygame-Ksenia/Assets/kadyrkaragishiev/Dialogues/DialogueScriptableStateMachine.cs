using System;
using kadyrkaragishiev.ReactiveState.StateMachine;
using UnityEngine;

namespace kadyrkaragishiev.Dialogues
{
    [CreateAssetMenu(fileName = "DialogueScriptableStateMachine", menuName = "kadyrkaragishiev/Dialogues/DialogueScriptableStateMachine", order = 0)]
    public class DialogueScriptableStateMachine : ScriptableStateMachine<DialogueState>
    {
        [HideInInspector]
        public string Id;

        public event Action<DialogueScriptableStateMachine, int, int> ScoreUpdate;

        public void InvokeScoreUpdate(int ecologicDelta, int wellFareDelta) => ScoreUpdate?.Invoke(this, ecologicDelta,wellFareDelta);

        public void LoadState(DialogueState state)
        {
            this.state = state;
            InvokeStateChanged();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

#if UNITY_EDITOR
            _ = UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out Id, out long _);
#endif
        }
    }
}
