using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProductionGame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;
    [SerializeField] private PlacableObject ClothingProductionPrefab;
    [SerializeField] private Transform roadsContainer;

    private PlacableObject currentField = null;

    private bool isEnabled = false;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        minigameObj.SetActive(true);

        isEnabled = true;
    }

    void Update()
    {
        if (!isEnabled)
            return;

        if (currentField == null)
        {
            currentField = Instantiate(ClothingProductionPrefab, roadsContainer);
        }

        if (Input.GetMouseButtonDown(0))
            TryPlace();

        MovePlacable(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void MovePlacable(Vector2 pos)
    {
        if (currentField == null)
        {
            isEnabled = false;
            minigameObj.SetActive(false);

            EndGame();
            return;
        }

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

    private Vector3 GetWorldCoordinate(Vector3 position)
    {
        Vector3 mousePoint = new Vector3(position.x, position.y, 1);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
