using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;

    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextShowEffector messageEffector;

    [SerializeField] private GameObject continueAnswer;

    [SerializeField] private AnswerContainer answerPrefab;
    [SerializeField] private Transform answersContainer;
    private List<AnswerContainer> createdAnswers = new List<AnswerContainer>();

    private void Start()
    {
        DialogueController.Instance.CurrentDialogue.OnDialogueElementChanged.AddListener(OnDialogueChanged);
    }

    private void OnDestroy()
    {
        DialogueController.Instance.CurrentDialogue.OnDialogueElementChanged.RemoveListener(OnDialogueChanged);
    }

    private void OnDialogueChanged(DialogueElement from, DialogueElement to)
    {
        ShowDialogue(to);
    }

    private void ShowDialogue(DialogueElement element)
    {
        if(element.Type == DialogueElement.DialogueElementType.Minigame)
        {
            dialoguePanel.SetActive(false);
            return;
        }

        dialoguePanel.SetActive(true);

        ClearAnswers();

        characterImage.sprite = element.Character.Icon;
        characterName.text = element.Character.Title;

        message.text = element.Text;
        messageEffector.StartEffect();

        continueAnswer.SetActive(element.Type == DialogueElement.DialogueElementType.Message);

        if (element.Type == DialogueElement.DialogueElementType.Choice)
        {
            foreach (var item in element.Choices)
                CreateAnswer(item, element.Choices.IndexOf(item));
        }
    }

    private void ClearAnswers()
    {
        createdAnswers.ForEach(x => Destroy(x.gameObject));
        createdAnswers.Clear();
    }

    private void CreateAnswer(DialogueElementChoice element,int index)
    {
        AnswerContainer createdAnswer = Instantiate(answerPrefab, answersContainer);

        createdAnswer.Initialize(element, index);

        createdAnswers.Add(createdAnswer);
    }
}
