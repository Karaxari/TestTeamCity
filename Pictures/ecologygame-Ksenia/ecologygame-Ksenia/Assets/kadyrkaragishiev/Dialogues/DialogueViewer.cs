using kadyrkaragishiev.ReactiveState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace kadyrkaragishiev.Dialogues
{
    public class DialogueViewer : MonoBehaviour
    {
        [SerializeField] private DialogueStateHandlerBehaviour _stateHandlerBehaviour;

        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private TextMeshProUGUI message;
        
        private void OnEnable() => _stateHandlerBehaviour.StateEmitter.StateChanged += StateEmitterOnStateChanged;
        private void OnDisable() => _stateHandlerBehaviour.StateEmitter.StateChanged -= StateEmitterOnStateChanged;
        private void StateEmitterOnStateChanged(IStateEmitter<DialogueState> caller, DialogueState state)
        {
            characterImage.sprite = state.Character.GetCharacterImageState(state.characterState);
            characterName.text = state.Character.CharacterName;
            message.text = state.DialogueText;
        }
    }
}
