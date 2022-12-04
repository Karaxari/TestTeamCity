using System;
using System.Collections.Generic;
using UnityEngine;

public class ErasingPicture : MonoBehaviour
{
    public bool stillErasing = true;

    private Sprite mainSprite;
    private Texture2D m_Texture;
    private Color[] m_Colors;
    RaycastHit2D hit;
    SpriteRenderer spriteRend;
    Color zeroAlpha = Color.clear;
    public int erSize;
    public Vector2Int lastPos;
    public bool Drawing = false;

    List<Action<long>> onErasedHandlers;

    private long clearedPixelsCount;
    private int availabaleErasingAreaPercentage;
    private long availableErasingPixelsCount;

    //bool locked; //olc
    bool initialized;
    public long TotalPixelsCount
    {
        get => m_Colors.Length;
    }
    public bool Initialized { get => initialized; set => initialized = value; }
    public long AvailableErasingPixelsCount { get => availableErasingPixelsCount; set => availableErasingPixelsCount = value; }

    void Awake()
    {
        // availableErasingPixelsCount = 100;
        Debug.LogWarning("Awake of erasing picture");

        onErasedHandlers = new List<Action<long>>();

        spriteRend = gameObject.GetComponent<SpriteRenderer>();

        //bad code?
        if (mainSprite != null)
            spriteRend.sprite = mainSprite;
    }
    private void Start()
    {
        FitSprite();

        var tex = spriteRend.sprite.texture;
        m_Texture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        m_Texture.filterMode = FilterMode.Bilinear;
        m_Texture.wrapMode = TextureWrapMode.Clamp;
        m_Colors = tex.GetPixels();
        m_Texture.SetPixels(m_Colors);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f));

        availableErasingPixelsCount = m_Colors.Length / 100 * availabaleErasingAreaPercentage;
        clearedPixelsCount = 0;

        //locked = true;
        initialized = true;
    }

    void Update()
    { 
        if (Input.GetMouseButton(0))
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && availableErasingPixelsCount > 0) // && !locked)
            {
                UpdateTexture();
                Drawing = true;
            }
        }
        else
            Drawing = false;
        Debug.Log(availableErasingPixelsCount);
        if (availableErasingPixelsCount <= 0)
        {
            stillErasing = false;
            Debug.Log("availableErasingPixelsCount");
        }
    }


    private void FitSprite()
    {
        var height = 2 * Camera.main.orthographicSize;
        var width = height * Camera.main.aspect;

        //Setting sprite width and height
        float spriteWidth = width - (width * 0.5f);
        float spriteHeight = height - (height * 0.5f);
        spriteRend.size = new Vector2(spriteWidth, spriteHeight);

        //Setting BoxCollider2D width and heigh
        GetComponent<BoxCollider2D>().size = new Vector2(spriteWidth, spriteHeight);
    }

    public void Setup(int availableErasingAreaByPercentage, Sprite sprite = null)
    {
        //bad code?
        if (sprite != null)
            mainSprite = sprite;

       availableErasingPixelsCount = (long)(availableErasingPixelsCount * availableErasingAreaByPercentage / 100f);
       this.availabaleErasingAreaPercentage = availableErasingAreaByPercentage;
    }

    //old
    //public void Unlock()
    //{
    //    locked = false;
    //}

    private Texture2D CopyTexture(Texture2D original)
    {
        Texture2D copy = new Texture2D(original.width, original.height, TextureFormat.ARGB32, false);
        copy.filterMode = FilterMode.Bilinear;
        copy.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = original.GetPixels();
        copy.SetPixels(colors);
        copy.Apply();

        return copy;
    }

    //public void OnDrawing(RaycastHit2D hit)
    //{
    //    this.hit = hit; 
    //    Drawing = true;
    //    UpdateTexture();
    //}
    //public void OnDrawingEnd()
    //{
    //    Drawing = false;
    //}

    public void AddOnErasingHandler(Action<long> handler)
    {
        onErasedHandlers.Add(handler);
    }


    public void UpdateTexture()
    {
        clearedPixelsCount = 0;

        int w = m_Texture.width;
        int h = m_Texture.height;
        var mousePos = hit.point - (Vector2)hit.collider.bounds.min;
        mousePos.x *= w / hit.collider.bounds.size.x;
        mousePos.y *= h / hit.collider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        Vector2Int start = new Vector2Int();
        Vector2Int end = new Vector2Int();
        if (!Drawing)
            lastPos = p;
        start.x = Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - erSize, 0, w);
        start.y = Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - erSize, 0, h);
        end.x = Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + erSize, 0, w);
        end.y = Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + erSize, 0, h);
        Vector2 dir = p - lastPos;
        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;
                if (Drawing)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }
                if ((pixel - linePos).sqrMagnitude <= erSize * erSize)
                {
                    if (m_Colors[x + y * w].a != 0f)
                    {
                        clearedPixelsCount++;
                        //availableErasingPixelsCount--;
                    }

                    m_Colors[x + y * w] = zeroAlpha;
                }
            }
        }
        lastPos = p;
        m_Texture.SetPixels(m_Colors);
        //Debug.Log("Cleared colors count: " + clearedPixelsCount);
        m_Texture.Apply();
        spriteRend.sprite = Sprite.Create(m_Texture, spriteRend.sprite.rect, new Vector2(0.5f, 0.5f));

        availableErasingPixelsCount -= clearedPixelsCount*2;

        foreach (var handler in onErasedHandlers)
        {
            handler.Invoke(clearedPixelsCount);
        }
    }
}