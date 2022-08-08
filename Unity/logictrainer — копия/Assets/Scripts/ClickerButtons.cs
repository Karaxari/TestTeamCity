using System;
using UnityEngine;
using UnityEngine.UI;

public class ClickerButtons : MonoBehaviour
{
    MergeController mergeController;
    FormulaController formulaController;
    DrawingController drawingController;

    void Start()
    {
        mergeController = FindObjectOfType<MergeController>();
        formulaController = FindObjectOfType<FormulaController>();
        drawingController = FindObjectOfType<DrawingController>();
    }

    public void clickButton()
    {
        //Debug.Log(name);
        string str = this.name.Replace("Button_", "");
        mergeController.checkResult(Int32.Parse(str));
    }

    public void cancelButton()
    {
        Debug.Log(name);
        string str = this.name.Replace("Button_", "");
        formulaController.cancelTerm(Int32.Parse(str));
    }

    public void choiceButton()
    {
        Debug.Log(name);
        string str = this.name.Replace("Button_", "");
        formulaController.choiceAnswer(Int32.Parse(str));
    }

    public void checkButton()
    {
        //Debug.Log(name);
        drawingController.checkResult(this.gameObject.GetComponentInChildren<Image>().sprite.name);
    }
}
