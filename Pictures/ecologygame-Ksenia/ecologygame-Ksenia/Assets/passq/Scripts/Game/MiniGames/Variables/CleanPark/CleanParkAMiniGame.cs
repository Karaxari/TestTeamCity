using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CleanParkAMiniGame : MinigameBase
{
    [SerializeField] private GameObject minigameObj;



    private bool isEnabled = false;

    [Header("First Step")]
    [SerializeField] private GameObject bench;
    [SerializeField] ErasingController erasingController;

    [Header("Second Step")]
    [SerializeField] private GameObject treesPlZone;
    [SerializeField] private int treesCount;
    [SerializeField] private PlacableObject treePrefab;
    [SerializeField] private GameObject treesCountPanel;
    [SerializeField] private TMP_Text treesCountText;

    bool isStage2Passed = false;

    [Header("Third Step")]
    [SerializeField] private GameObject swingsPlZone;
    [SerializeField] private int swingsCount;
    [SerializeField] private PlacableObject swingPrefab;
    [SerializeField] private GameObject swingCountPanel;
    [SerializeField] private TMP_Text swingsCountText;

    [Header("Common")]
    [SerializeField] private Transform fieldContainer;
    private PlacableObject currentPlObj = null;


    protected override void OnGameStarted()
    {
        base.OnGameStarted();
        minigameObj.SetActive(true);
        treesCountPanel.transform.parent.gameObject.SetActive(true);


        treesCountPanel.SetActive(false);
        treesPlZone.SetActive(false);
        swingsPlZone.SetActive(false);
        swingCountPanel.SetActive(false);

        StartCoroutine(StageProcess());


    }

    IEnumerator StageProcess()
    {
        //Stage1
        bench.SetActive(true);


        while (erasingController.crntErasingPicture.stillErasing)
        {
            yield return null;
        }
        
        //Stage2
        isEnabled = true;
        bench.SetActive(false);

        treesPlZone.SetActive(true);
        treesCountPanel.SetActive(true);
        UpdatePlacableCountUI(treesCountText, treesCount);

        while (!isStage2Passed)
        {
            yield return null;
        }
        treesCountPanel.SetActive(false);
        treesPlZone.SetActive(false);

        swingsPlZone.SetActive(true);
        swingCountPanel.SetActive(true);
        
    }


    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        swingCountPanel.SetActive(false);
        swingsPlZone.SetActive(false);
        treesCountPanel.transform.parent.gameObject.SetActive(false);

        isEnabled = false;
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (currentPlObj == null)
            GetPlacable();

        MovePlacable(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButtonDown(0))
            TryPlace();
    }

    private void GetPlacable()
    {
        if (treesCount <= 0)
        {
            isStage2Passed = true;
        }
        else
        {
            currentPlObj = Instantiate(treePrefab, fieldContainer);

            UpdatePlacableCountUI(treesCountText, treesCount);
            treesCount--;
        }
        if (isStage2Passed)
        {
            if (swingsCount <= 0)
            {
                isStage2Passed = true;
                EndGame();
                return;
            }
            currentPlObj = Instantiate(swingPrefab, fieldContainer);

            UpdatePlacableCountUI(swingsCountText, swingsCount);
            swingsCount--;
        }

    }

    private void MovePlacable(Vector2 pos)
    {
        if (currentPlObj == null)
            return;

        currentPlObj.transform.position = pos;
    }

    private void TryPlace()
    {
        if (currentPlObj == null)
            return;

        if (!currentPlObj.TryPlace())
            return;

        currentPlObj = null;
    }

    private void UpdatePlacableCountUI(TMP_Text plText, int plCount)
    {
        plText.text = $"x{plCount}";
    }
}
