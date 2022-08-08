using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSwitch : MonoBehaviour
{
    public Sprite FormulaBg;
    public Sprite LabyrinthBg;
    public Sprite MergeBg;
    public Sprite UniqueFigureBg;

    private Sprite defaultBg;
    private Image img;
    private float Speed = 10f;
    private float key = 0f;

    private void Start()
    {
        img = GetComponent<Image>();
        defaultBg = img.sprite;
    }

    public void ResetBG()
    {
        img.sprite = defaultBg;
    }
   
    //#edit. Add handlers code
    public void SwitchBG(string mode)
    {
        Debug.Log("SwitchBG");
        switch (mode)
        {
            case GameModes.formula: img.sprite = FormulaBg; key = 0f;
                break;
            case GameModes.labyrinth: img.sprite = LabyrinthBg; key = 0f;
                break;
            case GameModes.merge: img.sprite = MergeBg; key = 0f;
                break;
            case GameModes.uniqueDrawing: img.sprite = UniqueFigureBg; key = 0f;
                break;
            default: img.sprite = defaultBg; key = 1f;
                break;
        }
        StartCoroutine(LoadGameBackground());
    }

    IEnumerator LoadGameBackground()
    {
        var color = img.color;
        color.a = key;
        for (int i=0; i<100; i++)
        {
            if (key == 0)
                color.a += Speed * Time.deltaTime;
            else
                color.a -= Speed * Time.deltaTime;

            color.a = Mathf.Clamp(color.a, 0, 1);

            //Debug.Log(color.a);

            img.color = color;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
