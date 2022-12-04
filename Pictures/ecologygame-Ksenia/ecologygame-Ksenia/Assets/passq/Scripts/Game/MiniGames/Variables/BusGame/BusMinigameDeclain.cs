using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BusMinigameDeclain : MinigameBase

{
    [SerializeField] GameObject mapGO;

    [SerializeField] private List<RoadPointTapable> tapableBuses;

    [SerializeField] float busActiveDelay;
    [SerializeField] float busShowDelay;

    [SerializeField] int needCountOfTaps;
    int crntCountOfTaps;

    IEnumerator showingBusesCorutine;

    public void IncreaseTapsCount(int tmp = 0)
    {
        crntCountOfTaps++;
        if (crntCountOfTaps == needCountOfTaps)
        {
            StopCoroutine(showingBusesCorutine);
            EndGame();
        }
    }

    protected override void OnGameStarted()
    {
        mapGO.SetActive(true);
        crntCountOfTaps = 0;
        base.OnGameStarted();

        TapController.Instance.SetEnable(true);

        showingBusesCorutine = ShowBuses();
        tapableBuses.ForEach(x =>
        {
            x.OnTapped.AddListener(IncreaseTapsCount);
            x.gameObject.transform.localScale = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
        });
        StartCoroutine(showingBusesCorutine);
    }

    protected override void OnGameEnded()
    {
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        StopAllCoroutines();
        mapGO.SetActive(false);
    }

    IEnumerator ShowBuses()
    {
        int randInt;
        while (true)
        {
            do { randInt = UnityEngine.Random.Range(0, tapableBuses.Count); }
            while (tapableBuses[randInt].isActive);

            tapableBuses[randInt].SetStartScale();
            tapableBuses[randInt].isActive = true;
            tapableBuses[randInt].activationCount++;
            StartCoroutine(HideGO(tapableBuses[randInt]));

            yield return new WaitForSeconds(busShowDelay);
        }
    }

    IEnumerator HideGO(RoadPointTapable go)
    {
        int actCount = go.activationCount;
        yield return null;
        yield return new WaitForSeconds(busActiveDelay);
        if (go.isActive && actCount == go.activationCount)
        {
            go.isActive = false;
            go.GetComponent<ScaleEffect>().Execute();
        }

    }
}
