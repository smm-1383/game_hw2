public abstract class Obstacle : Interactable
{
    bool hit;

    public override void OnSpawn()
    {
        hit = false;
    }

    public override void OnHit(CarController car)
    {
        if (hit) return;
        hit = true;
        car.Crash();
    }
}
