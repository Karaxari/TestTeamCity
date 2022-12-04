using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject gamePanel;

    public void Show()
    {
        endPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
}
