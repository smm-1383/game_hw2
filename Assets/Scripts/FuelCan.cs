public class FuelCan : Collectible
{
    public const float Amount = 30f;

    protected override void Collect(CarController car)
    {
        if (car.Fuel != null) car.Fuel.Refill(Amount);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFuel();
    }
}
