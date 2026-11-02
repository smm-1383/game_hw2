using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    [SerializeField] GameObject[] staticCars;
    [SerializeField] GameObject[] movingCars;
    [SerializeField] GameObject[] coins;
    [SerializeField] GameObject[] fuelCans;
    [SerializeField] GameObject pickupEffect;
    [SerializeField] Fuel fuel;
    [SerializeField] int prewarm = 4;
    [SerializeField] int coinPrewarm = 24;
    [SerializeField] float coinHeight = 1f;
    [SerializeField] float coinSpacing = 2.5f;
    [SerializeField] float effectTime = 1.5f;

    Pool[] staticPools;
    Pool[] movingPools;
    Pool[] coinPools;
    Pool[] fuelPools;
    Pool effectPool;
    readonly Queue<(GameObject go, float time)> playing = new Queue<(GameObject, float)>();
    readonly bool[] taken = new bool[3];
    readonly bool[] prevFree = { true, true, true };
    float sinceFuel;
    int quiet = 99;

    Pool[] MakePools(GameObject[] prefabs, int count)
    {
        var result = new Pool[prefabs.Length];
        for (int i = 0; i < result.Length; i++) result[i] = new Pool(prefabs[i], count, transform);
        return result;
    }

    void Awake()
    {
        Instance = this;
        staticPools = MakePools(staticCars, prewarm);
        movingPools = MakePools(movingCars, prewarm);
        coinPools = MakePools(coins, coinPrewarm);
        fuelPools = MakePools(fuelCans, 2);
        if (pickupEffect != null) effectPool = new Pool(pickupEffect, 4, transform);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        while (playing.Count > 0 && playing.Peek().time <= Time.time) effectPool.Return(playing.Dequeue().go);
    }

    public void PlayEffect(Vector3 pos, Color color)
    {
        if (effectPool == null) return;
        var go = effectPool.Get(pos, Quaternion.identity);
        var ps = go.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = color;
            ps.Play();
        }
        playing.Enqueue((go, Time.time + effectTime));
    }

    void Place(Pool[] pools, int lane, float z, float y)
    {
        if (pools.Length == 0) return;
        var go = pools[Random.Range(0, pools.Length)].Get(new Vector3(GameManager.LaneX(lane), y, z), Quaternion.identity);
        go.GetComponent<Interactable>().OnSpawn();
    }

    int FreeLane(int avoid)
    {
        int start = Random.Range(0, 3);
        for (int i = 0; i < 3; i++)
        {
            int lane = (start + i) % 3;
            if (!taken[lane] && lane != avoid) return lane;
        }
        return -1;
    }

    bool Reachable(int a, int b)
    {
        int free = 3 - a - b;
        for (int i = 0; i < 3; i++)
        {
            if (prevFree[i] && Mathf.Abs(i - free) <= 1) return true;
        }
        return false;
    }

    float FuelInterval()
    {
        var gm = GameManager.Instance;
        if (fuel == null || gm == null) return 200f;
        return FuelCan.Amount * 0.5f * gm.Speed / fuel.DrainPerSecond;
    }

    public void PopulateSegment(float zStart, float length)
    {
        float difficulty = GameManager.Instance != null ? GameManager.Instance.Difficulty : 0f;
        float mid = zStart + length * 0.5f;
        System.Array.Clear(taken, 0, 3);

        if (Random.value < Mathf.Lerp(0.5f, 0.9f, difficulty))
        {
            int a = Random.Range(0, 3);
            int b = -1;
            if (quiet >= 3 && Random.value < difficulty)
            {
                b = (a + Random.Range(1, 3)) % 3;
                if (!Reachable(a, b)) b = -1;
            }
            taken[a] = true;
            bool moving = b < 0 && movingPools.Length > 0 && Random.value < 0.3f + 0.3f * difficulty;
            if (moving)
            {
                Place(movingPools, a, mid, 0f);
                quiet = 0;
            }
            else
            {
                Place(staticPools, a, mid, 0f);
                quiet++;
            }
            if (b >= 0)
            {
                taken[b] = true;
                Place(staticPools, b, mid, 0f);
            }
        }
        else
        {
            quiet++;
        }
        for (int i = 0; i < 3; i++) prevFree[i] = !taken[i];

        sinceFuel += length;
        int fuelLane = -1;
        if (sinceFuel >= FuelInterval())
        {
            fuelLane = FreeLane(-1);
            if (fuelLane >= 0)
            {
                Place(fuelPools, fuelLane, zStart + length * 0.25f, coinHeight);
                sinceFuel = 0f;
            }
        }

        int coinLane = FreeLane(fuelLane);
        if (coinLane >= 0 && Random.value < 0.5f)
        {
            int n = Random.Range(3, 7);
            float z = zStart + Random.Range(0f, Mathf.Max(0f, length - n * coinSpacing));
            for (int i = 0; i < n; i++) Place(coinPools, coinLane, z + i * coinSpacing, coinHeight);
        }
    }
}
