using UnityEngine;

public class Fireworks : MonoBehaviour
{
    [SerializeField] ParticleSystem[] bursts;
    [SerializeField] float minInterval = 0.8f;
    [SerializeField] float maxInterval = 2.5f;
    [SerializeField] float doubleChance = 0.3f;

    float timer;

    void Start()
    {
        timer = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (bursts.Length == 0 || (gm != null && gm.State != GameState.Playing && gm.State != GameState.Menu)) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(minInterval, maxInterval);
        Launch();
        if (Random.value < doubleChance) Launch();
    }

    void Launch()
    {
        int start = Random.Range(0, bursts.Length);
        ParticleSystem ps = null;
        for (int i = 0; i < bursts.Length; i++)
        {
            var candidate = bursts[(start + i) % bursts.Length];
            if (!candidate.isPlaying)
            {
                ps = candidate;
                break;
            }
        }
        if (ps == null) return;

        float z = Random.Range(50f, 90f);
        ps.transform.position = new Vector3(Random.Range(-40f, 40f), Random.Range(12f, 28f), z);
        var color = Color.HSVToRGB(Random.value, 0.8f, 1f) * Random.Range(4f, 7f);
        color.a = 1f;
        var main = ps.main;
        main.startColor = color;
        ps.Play();

        float far = Mathf.InverseLerp(50f, 90f, z);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFirework(Mathf.Lerp(0.3f, 0.7f, far), Mathf.Lerp(0.9f, 0.5f, far));
    }
}
