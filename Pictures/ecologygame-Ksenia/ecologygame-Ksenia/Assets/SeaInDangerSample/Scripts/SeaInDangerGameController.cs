using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaInDangerGameController : MonoBehaviour
{
    public GameObject MessagePanel;


    // Start is called before the first frame update
    void Start()
    {
        ShowPanel(MessagePanel);
    }

    private void ShowPanel(GameObject panel) => panel.gameObject.SetActive(true);
}
