using System.Collections.Generic;
using kadyrkaragishiev.ReactiveState;
using UnityEngine;

namespace kadyrkaragishiev.Dialogues
{
    public abstract class DialogueStateHandlerBehaviour : StateHandlerBehaviour<DialogueState>
    {

        public List<DialogueState> HandledStates;

        protected abstract void OnStateHandled(DialogueState state);

        protected virtual void OnStateNotHandled(DialogueState state) { }

        protected virtual void OnStateEntered(DialogueState state) { }

        protected virtual void OnStateExited(DialogueState state) { }

        protected virtual void OnStatePersisted() { }

        protected virtual void OnStatePersisted(DialogueState state) { }

        private DialogueState _previousState;

        public void RequestStateHelper(DialogueState state) => _ = StateEmitter.RequestState(state);

        private bool _callOnStateEnteredNextUpdate;
        private bool _callOnStateExitedNextUpdate;

        protected override void OnStateChanged(IStateEmitter<DialogueState> caller, DialogueState state)
        {
            _callOnStateEnteredNextUpdate = false;
            _callOnStateExitedNextUpdate = false;

            if (HandledStates.Contains(state)) OnStateHandled(state);
            else OnStateNotHandled(state);

            if (_previousState == state) return;

            if (HandledStates.Contains(state)) _callOnStateEnteredNextUpdate = true;
            else if (HandledStates.Contains(_previousState)) _callOnStateExitedNextUpdate = true;

            _previousState = state;
        }

        private void Update()
        {
            if (_callOnStateEnteredNextUpdate) OnStateEntered(_previousState);
            if (_callOnStateExitedNextUpdate) OnStateExited(_previousState);

            if (HandledStates.Contains(_previousState)) OnStatePersisted(_previousState);
            _callOnStateEnteredNextUpdate = false;
            _callOnStateExitedNextUpdate = false;
        }

        protected virtual void Awake() => StateEmitter.StateChanged += OnStateChanged;

        protected virtual void OnDestroy() => StateEmitter.StateChanged -= OnStateChanged;
    }
}
