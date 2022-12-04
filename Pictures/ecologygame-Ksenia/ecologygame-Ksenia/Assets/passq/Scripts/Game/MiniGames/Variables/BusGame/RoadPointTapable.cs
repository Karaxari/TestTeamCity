using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadPointTapable : BaseMapTapable
{
    [SerializeField] private UnityEvent<Transform> onTappedVariable = new UnityEvent<Transform>();

    Vector3 startObjScale;
    [HideInInspector] public bool isActive;
    public int activationCount;

    private void Awake()
    {
        activationCount = 0;
        isActive = false;
        startObjScale = gameObject.transform.localScale;
    }
    public Vector3 GetStartScale()
    {
        return startObjScale;
    }

    public void SetStartScale()
    {
        transform.localScale = startObjScale;
    }

    public void SetActive(bool activity)
    {
        isActive = activity;
    }

    public UnityEvent<Transform> OnTappedVariable { get { return onTappedVariable; } }
    protected override void VariableAction()
    {
        OnTappedVariable.Invoke(gameObject.transform);
    }


}
