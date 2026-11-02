using UnityEngine;

public abstract class Collectible : Interactable
{
    [SerializeField] Color effectColor = Color.yellow;
    [SerializeField] float spinSpeed = 120f;

    bool taken;

    public override void OnSpawn()
    {
        taken = false;
    }

    protected override void Update()
    {
        base.Update();
        if (Playing) transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    public override void OnHit(CarController car)
    {
        if (taken) return;
        taken = true;
        Collect(car);
        if (Spawner.Instance != null) Spawner.Instance.PlayEffect(transform.position, effectColor);
        Release();
    }

    protected abstract void Collect(CarController car);
}
