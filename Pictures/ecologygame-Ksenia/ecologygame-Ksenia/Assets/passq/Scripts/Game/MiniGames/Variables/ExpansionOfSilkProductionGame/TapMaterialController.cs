using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TapMaterialController : MonoBehaviour
{
    [SerializeField] private bool infinityTapable = false;
    [SerializeField] private int tapsToComplete;

    [SerializeField] private UnityEvent<int> onTapped = new UnityEvent<int>();
    [SerializeField] private UnityEvent onCompleteTapping = new UnityEvent();

    private int tapCount = 0;

    public UnityEvent<int> OnTapped { get { return onTapped; } }
    public UnityEvent OnCompleteTapping { get { return onCompleteTapping; } }

    public bool IsCompleted => !infinityTapable && tapCount >= tapsToComplete;

    protected virtual bool CanTap()
    {
        if (!infinityTapable && tapCount >= tapsToComplete)
            return false;

        return true;
    }

    public void OnTap()
    {
        if (!CanTap())
            return;

        tapCount++;

        onTapped?.Invoke(tapCount);

        if (!infinityTapable && tapCount >= tapsToComplete)
            onCompleteTapping?.Invoke();
    }

    public void SetTapsToComplete(int taps)
    {
        tapsToComplete = taps;
    }

    public int GetTapsToComplete()
    {
        //Debug.LogError(tapCount + " �� " + tapsToComplete);
        return tapCount;
    }
}
