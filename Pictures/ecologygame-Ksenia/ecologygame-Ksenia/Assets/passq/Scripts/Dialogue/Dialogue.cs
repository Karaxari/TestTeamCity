using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New dialogue", menuName = "passq/Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeReference] [SerializeField] private List<DialogueTransition> transitions = new List<DialogueTransition>();

    [SerializeField] private DialogueElement startElement;

    private DialogueElement currentElement = null;

    public readonly UnityEvent<DialogueElement, DialogueElement> OnDialogueElementChanged = new UnityEvent<DialogueElement, DialogueElement>();

    public readonly UnityEvent OnEnded = new UnityEvent();

    public void StartElement()
    {
        SetElement(startElement);
    }

    public void NextElement()
    {
        if (currentElement == null)
            SetElement(startElement);
        else
            NextElement(transitions.Select(x => new { transition = x, element = x.GetNextElement(currentElement) }).FirstOrDefault(x => x.element != null)?.element ?? null);
    }

    public void NextElement(DialogueElement target)
    {
        if (target == null)
        {
            OnEnded?.Invoke();
            return;
        }

        if (!transitions.Any(x => x.CanTransit(currentElement, target)))
        {
            OnEnded?.Invoke();
            return;
        }

        SetElement(target);
    }

    public void NextElement(DialogueElementChoice choice)
    {
        NextElement(choice.SelectedElement);
    }

    private void SetElement(DialogueElement element)
    {
        var lastElement = currentElement;

        currentElement = element;

        currentElement.Enter();
        currentElement.Execute();

        lastElement?.Exit();

        OnDialogueElementChanged?.Invoke(lastElement, element);
    }

#if UNITY_EDITOR

    [ContextMenu("Transitions/Default transition")]
    private void Editor_AddDefaultTransition()
    {
        transitions.Add(new DialogueTransition());

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Transitions/Parameter transition")]
    private void Editor_AddParameterTransition()
    {
        transitions.Add(new DialogueParameterTransition());

        UnityEditor.EditorUtility.SetDirty(this);
    }

#endif
}
