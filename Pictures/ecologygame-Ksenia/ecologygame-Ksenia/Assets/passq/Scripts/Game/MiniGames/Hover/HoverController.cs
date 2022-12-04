using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{
    public static HoverController Instance { get; private set; }

    private bool isEnabled = false;

    private void Awake()
    {
        Instance = this;
    }

    public void SetEnable(bool value)
    {
        isEnabled = value;
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (Input.GetMouseButton(0))
            ExecuteMapHover(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void ExecuteMapHover(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.001f);

        foreach (var item in hits)
            if (item.TryGetComponent<BaseMapHover>(out BaseMapHover hover))
                hover.OnHover();
    }
}
