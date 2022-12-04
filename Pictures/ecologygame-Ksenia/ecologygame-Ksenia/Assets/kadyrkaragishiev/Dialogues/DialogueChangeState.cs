using System;
using System.Collections;
using System.Collections.Generic;
using kadyrkaragishiev.Dialogues;
using UnityEngine;

public class DialogueChangeState : MonoBehaviour
{
    public DialogueScriptableStateMachine Dialogue;

    private List<DialogueState> _states;

    private void Start() => _states = Dialogue.PossibleStates;

    public void ChangeState()
    {
        _ = Dialogue.RequestState(
            _states[(_states.FindIndex(x => x == Dialogue.State) + 1) < _states.Count
                ? _states.FindIndex(x => x == Dialogue.State) + 1
                : _states.FindIndex(x => x == Dialogue.State)]);
    }

    [ContextMenu("ChangeOnSelect")]
    private void OnSelectEnter() => ChangeState();
}
