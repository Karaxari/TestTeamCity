using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItemID : MonoBehaviour
{
    public static GameObject GetObject(string id) => allItems[id] ?? null;

    private static Dictionary<string,GameObject> allItems = new Dictionary<string,GameObject>();

    [SerializeField] private bool removeOnDisable = false;

    [SerializeField] private string id;

    public void Initialize()
    {
        TryAdd();
    }

    private void Awake()
    {
        TryAdd();
    }

    private void OnDestroy()
    {
        TryRemove();
    }

    private void OnDisable()
    {
        if (!removeOnDisable)
            return;

        TryRemove();
    }

    private void OnEnable()
    {
        TryAdd();
    }

    private void TryRemove()
    {
        if (!allItems.ContainsKey(id))
            return;

        allItems.Remove(id);
    }

    private void TryAdd()
    {
        if (allItems.ContainsKey(id))
            return;

        allItems.Add(id, gameObject);
    }
}
