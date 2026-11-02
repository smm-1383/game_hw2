using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    readonly GameObject prefab;
    readonly Transform parent;
    readonly Stack<GameObject> free = new Stack<GameObject>();

    public Pool(GameObject prefab, int count, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        for (int i = 0; i < count; i++) free.Push(Make());
    }

    GameObject Make()
    {
        var go = Object.Instantiate(prefab, parent);
        go.SetActive(false);
        if (go.TryGetComponent(out Interactable item)) item.Despawn = _ => Return(go);
        return go;
    }

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        var go = free.Count > 0 ? free.Pop() : Make();
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go)
    {
        if (!go.activeSelf) return;
        go.SetActive(false);
        free.Push(go);
    }
}
