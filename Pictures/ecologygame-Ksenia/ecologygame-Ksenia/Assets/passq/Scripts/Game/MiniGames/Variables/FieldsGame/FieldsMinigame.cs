using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FieldsMinigame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;

    [SerializeField] private GameObject fieldsCountPanel;
    [SerializeField] private TMP_Text fieldsCountText;

    [SerializeField] private int fieldsCount;
    [SerializeField] private PlacableObject fieldPrefab;
    [SerializeField] private Transform fieldContainer;

    private PlacableObject currentField = null;

    private bool isEnabled = false;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        fieldsCountPanel.SetActive(true);
        minigameObj.SetActive(true);

        UpdateFieldsCountUI();

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

    private void GetPlacable()
    {
        if (fieldsCount <= 0)
        {
            EndGame();
            return;
        }

        currentField = Instantiate(fieldPrefab,fieldContainer);

        UpdateFieldsCountUI();
        fieldsCount--;
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
        fieldsCountText.text = $"x{fieldsCount}";
    }
}
