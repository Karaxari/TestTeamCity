using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BusMinigameAccept : MinigameBase
{

    [SerializeField] private List<RoadPointTapable> pointsTapables;

    [SerializeField] private LineRendererController lrController;

    [SerializeField] GameObject mapGO;
    [SerializeField] GameObject signsGO;

    [SerializeField] Material roadMaterial;
    [SerializeField] Transform lastPoint;
    [SerializeField] GameObject busToTranslate;

    [SerializeField] float timeForTranslatebus;
    [SerializeField] float busDelay;

    int crntActivePoint;
    protected override void OnGameStarted()
    {
        crntActivePoint = 0;
        base.OnGameStarted();

        mapGO.SetActive(true);
        signsGO.SetActive(true);
        TapController.Instance.SetEnable(true); ;

        pointsTapables.ForEach(x =>
        {
            x.OnCompleteTapping.AddListener(CheckTreesTapables);
            x.gameObject.transform.localScale = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
        });
        pointsTapables[crntActivePoint].SetStartScale();
        crntActivePoint++;
        pointsTapables.ForEach(x => x.OnTappedVariable.AddListener(OnTapped));
    }

    void OnTapped(Transform pointTrans)
    {
        lrController.AddPoint(pointTrans);
        if (crntActivePoint != pointsTapables.Count)
        {
            pointsTapables[crntActivePoint].SetStartScale();
            crntActivePoint++;
        }

    }

    protected override void OnGameEnded()
    {
        lrController.AddPoint(lastPoint);
        lrController.SetLineMaterial(roadMaterial);
        signsGO.SetActive(false);
        base.OnGameEnded();

        TapController.Instance.SetEnable(false);

        StopAllCoroutines();

        StartCoroutine(TranslateBus(timeForTranslatebus));
        pointsTapables.ForEach(x => x.OnCompleteTapping.RemoveListener(CheckTreesTapables));
    }

    private void CheckTreesTapables()
    {
        if (pointsTapables.Where(x => x.gameObject.activeInHierarchy).All(x => x.IsCompleted))
        { EndGame(); }
    }


    IEnumerator TranslateBus(float duration)
    {
        Vector3 startPosition;
        Vector3 targetPosition;
        float time;

        while (true)
        {

            int crntInd = 0, lastInd = lrController.GetPointsCount();

            while (crntInd != lastInd - 1)
            {
                time = 0;
                startPosition = lrController.GetPoint(crntInd).position;
                targetPosition = lrController.GetPoint(crntInd + 1).position;


                while (time < duration)
                {
                    busToTranslate.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                    time += Time.deltaTime;
                    yield return null;
                }
                busToTranslate.transform.position = targetPosition;
                crntInd++;

            }

            yield return new WaitForSeconds(busDelay);
        }
    }
}

