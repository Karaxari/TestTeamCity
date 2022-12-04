using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreserveMinigame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;
    [SerializeField] private GameObject zonesFencing;

    [SerializeField] private GameObject fieldsCountPanel;
    [SerializeField] private TMP_Text[] fieldsCountText;

    [SerializeField] private int indexPrefab;
    [SerializeField] private int[] fieldsCount;
    [SerializeField] private PlacableObject[] fieldPrefab;
    [SerializeField] private Transform fieldContainer;

    private PlacableObject currentField = null;

    private bool isEnabled = false;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        fieldsCountPanel.SetActive(true);
        minigameObj.SetActive(true);
        zonesFencing.SetActive(true);

        for (int i = 0; i < fieldsCount.Length; i++)
        {
            indexPrefab = i;
            UpdateFieldsCountUI();
        }
        SetIndex(0);

        isEnabled = true;
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        fieldsCountPanel.SetActive(false);
        minigameObj.SetActive(false);

        isEnabled = false;
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (currentField == null)
            GetPlacable();

        MovePlacable(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0))
            TryPlace();
    }

    private bool CheckCount()
    {
        foreach (int i in fieldsCount)
        {
            if (i > 0)
                return false;
            else if (fieldsCount[i] == 0)
                SetIndex(0);
        }
        return true;
    }

    public void SetIndex(int index)
    {
        if (index >= 0 && index < fieldsCount.Length && index < fieldPrefab.Length)
        {
            for (int i = index; i < fieldsCount.Length; i++)
            {
                if (fieldsCount[i] > 0)
                {
                    indexPrefab = i;
                    return;
                }
            }
        }

        //if (!CheckCount())
        //{
        //    SetIndex(0);
        //}
    }

    private void GetPlacable()
    {
        if (CheckCount())
        {
            EndGame();
            return;
        }

        currentField = Instantiate(fieldPrefab[indexPrefab], fieldContainer);
        fieldsCount[indexPrefab]--;
        UpdateFieldsCountUI();
    }

    private void MovePlacable(Vector2 pos)
    {
        if (currentField == null)
            return;

        currentField.transform.position = pos;
    }

    private void TryPlace()
    {
        if (currentField == null)
            return;

        if (!currentField.TryPlace())
            return;

        currentField = null;
    }

    private void UpdateFieldsCountUI()
    {
        fieldsCountText[indexPrefab].text = $"x{fieldsCount[indexPrefab]}";
    }
}
