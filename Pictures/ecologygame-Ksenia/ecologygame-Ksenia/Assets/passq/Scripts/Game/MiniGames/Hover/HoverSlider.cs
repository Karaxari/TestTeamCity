using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HoverSlider : MonoBehaviour
{
    [SerializeField] private UnityEvent<int> onIndexMoved = new UnityEvent<int>();

    [SerializeField] private UnityEvent onMaxIndexArrived = new UnityEvent();

    [SerializeField] private List<BaseMapHoverSlider> elements = new List<BaseMapHoverSlider>();

    private int lastIndex = 0;
    private int nextIndex => lastIndex + 1;

    private int maxIndex;

    public UnityEvent<int> OnIndexMoved { get { return onIndexMoved; } }
    public UnityEvent OnMaxIndexArrived { get { return onMaxIndexArrived; } }


    public bool CanHover(int index) => index <= nextIndex;

    private void Awake()
    {
        elements.ForEach(x => x.Initialize(this));

        maxIndex = elements.Max(x => x.SliderIndex);
    }

    public void MoveHover(int index)
    {
        lastIndex = index;

        onIndexMoved?.Invoke(lastIndex);

        if (maxIndex == index)
            onMaxIndexArrived?.Invoke();
    }
}
