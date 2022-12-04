using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set;}

    [SerializeField] private Dialogue currentDialogue;
    public Dialogue CurrentDialogue { get { return currentDialogue; } }

    public readonly UnityEvent<Dialogue> OnDialogueEnded = new UnityEvent<Dialogue>();

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        currentDialogue.OnEnded.AddListener(() => OnDialogueEnded?.Invoke(currentDialogue));
    }

    public void StartDialogue()
    {
        currentDialogue.StartElement();
    }

    public void SelectNext()
    {
        currentDialogue.NextElement();
    }

    public void SelectAnswer(DialogueElementChoice choice)
    {
        currentDialogue.NextElement(choice);
    }
}
