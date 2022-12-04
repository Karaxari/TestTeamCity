using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace kadyrkaragishiev.Dialogues
{
    [CreateAssetMenu(fileName = "DialogueCharacter", menuName = "kadyrkaragishiev/Dialogues/DialogueCharacter",
        order = 1)]
    public class DialogueCharacter : ScriptableObject
    {
        [SerializeField] private string characterName;
        public string CharacterName => characterName;
        [SerializeField] public List<CharacterStateHandlers> characterStateHandlersList;

        public Sprite GetCharacterImageState(CharacterStates state) =>
            characterStateHandlersList.Find(x => x._characterState == state)._characterSpriteState;
    }

    [Serializable]
    public struct CharacterStateHandlers
    {
        public CharacterStates _characterState;
        [JsonIgnore]
        public Sprite _characterSpriteState;
    }
}
