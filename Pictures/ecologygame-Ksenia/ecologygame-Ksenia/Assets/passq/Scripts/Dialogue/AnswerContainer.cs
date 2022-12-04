using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerContainer : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text text;

    private DialogueElementChoice choiseElement;

    public void Initialize(DialogueElementChoice element,int index)
    {
        choiseElement = element;

        numberText.text = $"{index + 1}.";
        text.text = element.Text;

        text.color = element.TextColor;
    }

    public void OnClick()
    {
        DialogueController.Instance.SelectAnswer(choiseElement);
    }
}
