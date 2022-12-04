using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapController : MonoBehaviour
{
    public static TapController Instance { get; private set; }

    [SerializeField] private float tapRadius = 0.05f;

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

        if (Input.GetMouseButtonDown(0))
            ExecuteMapTap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void ExecuteMapTap(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, tapRadius);

        foreach(var item in hits)
            if(item.TryGetComponent<BaseMapTapable>(out BaseMapTapable tapable))
                tapable.OnTap();
    }
}
