using System.Collections.Generic;
using kadyrkaragishiev.ReactiveState;
using UnityEngine;
using UnityEngine.UI;

namespace kadyrkaragishiev.Dialogues
{
    public class DialogueHandlerPanel : DialogueStateHandlerBehaviour
    {
        [SerializeField] private GameObject MoveNextDialogue;

        [SerializeField] private GameObject DialogueButtonHandler;
        protected override void OnStateHandled(DialogueState state) => throw new System.NotImplementedException();

        private void OnEnable() => StateEmitter.StateChanged += StateEmitterOnStateChanged;

        private void OnDisable() => StateEmitter.StateChanged -= StateEmitterOnStateChanged;

        private void StateEmitterOnStateChanged(IStateEmitter<DialogueState> caller, DialogueState state)
        {
            MoveNextDialogue.SetActive(state.isDialogue == false);
            DialogueButtonHandler.SetActive(state.isDialogue);
        }
    }
}
