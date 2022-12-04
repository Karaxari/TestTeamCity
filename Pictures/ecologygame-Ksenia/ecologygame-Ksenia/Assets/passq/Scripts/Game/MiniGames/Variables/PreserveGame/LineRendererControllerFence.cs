using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererControllerFence : MonoBehaviour
{
    LineRenderer lr;

    [SerializeField] List<Transform> points = new List<Transform>();
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    void Start()
    {
        lr.positionCount = points.Count;
    }

    public void SetLineMaterial(Material newMAt)
    {
        lr.material = newMAt;
    }
    public Transform GetPoint(int index)
    {
        return points[index];
    }
    public int GetPointsCount()
    {
        return points.Count;
    }

    public void AddPoint(Transform addPointTransform)
    {
        points.Add(addPointTransform);
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }

    void Update()
    {
        /*  for (int i = 0; i < points.Count; i++)
          {
              lr.SetPosition(i, points[i].position); 
          }*/
    }
}
