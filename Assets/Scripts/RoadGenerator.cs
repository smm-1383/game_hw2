using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] segmentPrefabs;
    [SerializeField] Spawner spawner;
    [SerializeField] int segmentCount = 9;
    [SerializeField] int prewarm = 3;
    [SerializeField] float behindMargin = 25f;
    [SerializeField] float emptyDistance = 60f;
    [SerializeField] float idleSpeed = 6f;

    readonly List<GameObject> segments = new List<GameObject>();
    readonly Dictionary<GameObject, Pool> owners = new Dictionary<GameObject, Pool>();
    Pool[] pools;

    void Awake()
    {
        pools = new Pool[segmentPrefabs.Length];
        for (int i = 0; i < pools.Length; i++) pools[i] = new Pool(segmentPrefabs[i], prewarm, transform);
    }

    void Start()
    {
        while (segments.Count < segmentCount) AddSegment();
    }

    float LengthOf(GameObject go)
    {
        return go.GetComponent<RoadSegment>().Length;
    }

    void AddSegment()
    {
        float z = -behindMargin;
        if (segments.Count > 0)
        {
            var last = segments[segments.Count - 1];
            z = last.transform.position.z + LengthOf(last);
        }
        var pool = pools[Random.Range(0, pools.Length)];
        var go = pool.Get(new Vector3(0f, 0f, z), Quaternion.identity);
        owners[go] = pool;
        segments.Add(go);
        if (spawner != null && z >= emptyDistance) spawner.PopulateSegment(z, LengthOf(go));
    }

    void Update()
    {
        var gm = GameManager.Instance;
        float speed = idleSpeed;
        if (gm != null) speed = gm.State == GameState.Playing ? gm.Speed : 0f;
        foreach (var seg in segments) seg.transform.position += Vector3.back * speed * Time.deltaTime;

        var first = segments[0];
        if (first.transform.position.z + LengthOf(first) < -behindMargin)
        {
            owners[first].Return(first);
            segments.RemoveAt(0);
            AddSegment();
        }
    }
}
