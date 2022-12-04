using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClothingProductionMinigame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;
    [SerializeField] private PlacableObject ClothingProductionPrefab;
    [SerializeField] private Transform roadsContainer;
    [SerializeField] private Material roadMaterial;

    private PlacableObject currentField = null;

    private int fieldsCount = 1;
    private bool isEnabled = false;
    private Vector3 position;
    private Vector3 lastPosition;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        minigameObj.SetActive(true);

        isEnabled = true;
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        minigameObj.SetActive(false);

        isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
            return;

        if (currentField == null)
            GetPlacable();

        MovePlacable(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = position;
            position = GetWorldCoordinate(Input.mousePosition);

            if (fieldsCount >= 0)
            {
                position = new Vector3(position.x - 0.4f, position.y - 0.32f, position.z);
                TryPlace();
            }
            else
            {
                DrawLine(position, lastPosition, Color.red, 0.5f);
            }

            float dist = Vector2.Distance(new Vector2(-5.43f, 0.33f), new Vector2(position.x, position.y));
            if (dist < 0.2f)
            {
                EndGame();
            }
        }
    }

    private void GetPlacable()
    {
        if (fieldsCount <= 0)
        {
            fieldsCount--;
            return;
        }

        currentField = Instantiate(ClothingProductionPrefab, roadsContainer);
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

    private Vector3 GetWorldCoordinate(Vector3 position)
    {
        Vector3 mousePoint = new Vector3(position.x, position.y, 1);
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.transform.parent = roadsContainer;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.material = roadMaterial;
        //lr.SetColors(color, color);
        lr.SetWidth(0.25f, 0.25f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }
}
