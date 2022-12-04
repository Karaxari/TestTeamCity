using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFood : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;

    public void ClickFood()
    {
        StartCoroutine(directPathMoving());
    }

    IEnumerator directPathMoving()
    {
        int stepMove = 10;

        float cof_x = targetPoint.position.x - transform.position.x;
        float cof_y = targetPoint.position.y - transform.position.y;

        cof_x = cof_x / stepMove;
        cof_y = cof_y / stepMove;

        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(cof_x, cof_y, 0);
        }

        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
