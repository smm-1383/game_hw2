using UnityEngine;

public class Fuel : MonoBehaviour
{
    [SerializeField] float max = 100f;
    [SerializeField] float drainPerSecond = 2f;
    [SerializeField] float drainGrowth = 0.5f;

    public float Current { get; private set; }
    public float Normalized => Current / max;

    public float DrainPerSecond
    {
        get
        {
            float difficulty = GameManager.Instance != null ? GameManager.Instance.Difficulty : 0f;
            return drainPerSecond * (1f + difficulty * drainGrowth);
        }
    }

    void Awake()
    {
        Current = max;
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Playing) return;
        Current = Mathf.Max(0f, Current - DrainPerSecond * Time.deltaTime);
        if (Current <= 0f) gm.GameOver();
    }

    public void Refill(float amount)
    {
        Current = Mathf.Min(max, Current + amount);
    }
}
