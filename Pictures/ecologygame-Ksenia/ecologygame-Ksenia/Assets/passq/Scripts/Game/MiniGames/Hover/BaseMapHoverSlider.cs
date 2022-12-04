using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapHoverSlider : BaseMapHover
{
    [SerializeField] private int sliderIndex = 0;

    private HoverSlider ownSlider;

    public int SliderIndex { get { return sliderIndex; } }

    protected override bool CanHover()
    {
        if (!base.CanHover())
            return false;

        if(!ownSlider.CanHover(sliderIndex))
            return false;

        return true;
    }

    public void Initialize(HoverSlider ownSlider)
    {
        this.ownSlider = ownSlider;
    }

    public override void OnHover()
    {
        if (!CanHover())
            return;

        base.OnHover();

        ownSlider.MoveHover(sliderIndex);
    }
}
