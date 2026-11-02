using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected float despawnZ = -15f;

    public Action<Interactable> Despawn;

    protected bool Playing
    {
        get { return GameManager.Instance != null && GameManager.Instance.State == GameState.Playing; }
    }

    protected virtual void Update()
    {
        if (!Playing) return;
        transform.position += Vector3.back * GameManager.Instance.Speed * Time.deltaTime;
        if (transform.position.z < despawnZ) Release();
    }

    protected void Release()
    {
        if (Despawn != null) Despawn(this);
        else gameObject.SetActive(false);
    }

    public virtual void OnSpawn()
    {
    }

    public abstract void OnHit(CarController car);
}
