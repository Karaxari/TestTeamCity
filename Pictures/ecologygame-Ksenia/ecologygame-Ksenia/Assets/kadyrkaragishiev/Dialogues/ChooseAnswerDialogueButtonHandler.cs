using System;
using UnityEngine;
using UnityEngine.UI;

namespace kadyrkaragishiev.Dialogues
{
    public class ChooseAnswerDialogueButtonHandler : MonoBehaviour
    {
        [SerializeField] private DialogueScriptableStateMachine _stateMachine;
        [SerializeField] private readonly bool isFirst;
        private void OnEnable() => SetButtonTransition();

        private void SetButtonTransition()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(AddTransition);
        }

        private void AddTransition() =>
            _ = _stateMachine.RequestState(isFirst == true
                ? _stateMachine.State.FirstChooseState
                : _stateMachine.State.SecondChooseState);
    }
}
