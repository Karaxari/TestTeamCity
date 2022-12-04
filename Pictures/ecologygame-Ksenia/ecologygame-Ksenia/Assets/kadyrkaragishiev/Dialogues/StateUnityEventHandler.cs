using UnityEngine.Events;

namespace kadyrkaragishiev.Dialogues
{
    public class StateUnityEventHandler : DialogueStateHandlerBehaviour
    {
        public UnityEvent<DialogueState> StateHandled;

        public UnityEvent<DialogueState> StateNotHandled;

        public UnityEvent<DialogueState> StateEntered;

        public UnityEvent<DialogueState> StateExited;

        public UnityEvent<DialogueState> StatePersisted;

        protected override void OnStateEntered(DialogueState state) => StateEntered?.Invoke(state);

        protected override void OnStateExited(DialogueState state) => StateExited?.Invoke(state);

        protected override void OnStateHandled(DialogueState state) => StateHandled?.Invoke(state);

        protected override void OnStateNotHandled(DialogueState state) => StateNotHandled?.Invoke(state);

        protected override void OnStatePersisted(DialogueState state) => StatePersisted?.Invoke(state);
    }
}
