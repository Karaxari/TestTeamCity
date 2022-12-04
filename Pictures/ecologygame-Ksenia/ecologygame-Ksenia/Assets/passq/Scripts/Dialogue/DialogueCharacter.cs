using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New dialogue character", menuName = "passq/Dialogue character")]
public class DialogueCharacter : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private Sprite icon;

    public string Title { get { return title; } }
    public Sprite Icon { get { return icon; } }
}
