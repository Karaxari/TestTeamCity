using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New dialogue element", menuName = "passq/Dialogue element")]
public class DialogueElement : ScriptableObject
{
    public enum DialogueElementType
    {
        Message,
        Choice,
        Minigame
    }

    [SerializeField] private DialogueElementType type;

    [TextArea]
    [SerializeField] private string text;
    [SerializeField] private DialogueCharacter character;

    [SerializeField] private List<DialogueElementChoice> choices = new List<DialogueElementChoice>();

    [SerializeReference]
    [SerializeField] private List<DialogueElementPreprocessor> enterPreprocessors = new List<DialogueElementPreprocessor>();
    [SerializeReference]
    [SerializeField] private List<DialogueElementPreprocessor> executePreprocessors = new List<DialogueElementPreprocessor>();
    [SerializeReference]
    [SerializeField] private List<DialogueElementPreprocessor> exitPreprocessors = new List<DialogueElementPreprocessor>();

    public DialogueElementType Type { get { return type; } }
    public string Text { get { return text; } }
    public DialogueCharacter Character { get { return character; } }
    public List<DialogueElementChoice> Choices { get { return choices; } }

    public void Enter()
    {
        OnEnter();
    }

    public void Execute()
    {
        OnExecute();
    }

    public void Exit()
    {
        OnExit();
    }

    protected virtual void OnEnter()
    {
        enterPreprocessors.ForEach(x => x.Execute());
    }

    protected virtual void OnExecute()
    {
        executePreprocessors.ForEach(x => x.Execute());
    }

    protected virtual void OnExit()
    {
        exitPreprocessors.ForEach(x => x.Execute());
    }

#if UNITY_EDITOR
    [ContextMenu("Preprocessors/Enter/Parameter preprocessor")]
    private void Editor_AddEnterParameter()
    {
        enterPreprocessors.Add(new ParameterPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Preprocessors/Exit/Parameter preprocessor")]
    private void Editor_AddExitParameter()
    {
        exitPreprocessors.Add(new ParameterPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Preprocessors/Execute/Parameter preprocessor")]
    private void Editor_AddExecuteParameter()
    {
        executePreprocessors.Add(new ParameterPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }


    [ContextMenu("Preprocessors/Enter/Item enabler preprocessor")]
    private void Editor_AddEnterEvent()
    {
        enterPreprocessors.Add(new ItemEnabledPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Preprocessors/Exit/Item enabler preprocessor")]
    private void Editor_AddExitEvent()
    {
        exitPreprocessors.Add(new ItemEnabledPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Preprocessors/Execute/Item enabler preprocessor")]
    private void Editor_AddExecuteEvent()
    {
        executePreprocessors.Add(new ItemEnabledPreprocessor());

        UnityEditor.EditorUtility.SetDirty(this);
    }

#endif
}

[System.Serializable]
public class DialogueElementChoice
{
    [SerializeField] private string text;
    [SerializeField] private DialogueElement selectedElement;
    [SerializeField] private Color32 textColor;

    public string Text { get { return text; } }
    public DialogueElement SelectedElement { get { return selectedElement; } }
    public Color32 TextColor { get { return textColor; } }
}
