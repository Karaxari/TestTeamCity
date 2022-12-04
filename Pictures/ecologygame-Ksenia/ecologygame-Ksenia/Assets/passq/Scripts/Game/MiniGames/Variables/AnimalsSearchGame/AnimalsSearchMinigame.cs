using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimalsSearchMinigame : MinigameBase
{
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private Image ImagePanel;
    [SerializeField] private TMP_Text TextPanel;
    [SerializeField] private GameObject InfoPanel;

    [SerializeField] private GameObject[] AnimalButton;
    [SerializeField] private Sprite[] AnimalImage;
    [SerializeField] private string[] AnimalInfo;

    private int indexAnimal;
    private int countAnimal;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        countAnimal = 0;
        UIPanel.SetActive(true);
    }
    public void ChooseAnimal(int index)
    {
        if (index < AnimalImage.Length && index < AnimalInfo.Length)
        {
            indexAnimal = index;
            ImagePanel.sprite = AnimalImage[indexAnimal];
            TextPanel.text = AnimalInfo[indexAnimal];

            InfoPanel.SetActive(true);
        }
    }

    public void ChooseNext()
    {
        AnimalButton[indexAnimal].SetActive(false);
        countAnimal++;
        InfoPanel.SetActive(false);

        if (countAnimal >= AnimalButton.Length)
        {
            UIPanel.SetActive(false);
            EndGame();
        }
    }
}
