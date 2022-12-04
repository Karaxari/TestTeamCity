using System.Diagnostics;
using MyBox;
using UnityEngine;

namespace kadyrkaragishiev.Dialogues
{
    [CreateAssetMenu(fileName = "DialogueState", menuName = "kadyrkaragishiev/Dialogues/DialogueState", order = 0)]
    public class DialogueState : ScriptableObject
    {
        [HideInInspector] public string Id;

        public string DialogueText;
        public DialogueCharacter Character;
        public CharacterStates characterState;
        public int ChangeEcologic;
        public int ChangeWellFare;

        public bool isDialogue = false;
        
        [ConditionalField("isDialogue")]
        public DialogueState FirstChooseState;
        [ConditionalField("isDialogue")]
        public DialogueState SecondChooseState;

#if UNITY_EDITOR
        private void OnValidate() =>
            _ = UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out Id, out long _);
#endif
    }
}
