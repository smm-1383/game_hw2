using UnityEngine;

public class Coin : Collectible
{
    [SerializeField] float bobHeight = 0.15f;
    [SerializeField] float bobSpeed = 3f;

    float baseY;

    public override void OnSpawn()
    {
        base.OnSpawn();
        baseY = transform.position.y;
    }

    protected override void Update()
    {
        base.Update();
        if (!Playing) return;
        var pos = transform.position;
        pos.y = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = pos;
    }

    protected override void Collect(CarController car)
    {
        var gm = GameManager.Instance;
        gm.AddCoin(Mathf.Max(1, Mathf.FloorToInt(gm.Speed * 3.6f / 2f)));
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoin();
    }
}
