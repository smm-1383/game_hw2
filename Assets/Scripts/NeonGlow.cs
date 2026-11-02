using UnityEngine;

public class NeonGlow : MonoBehaviour
{
    [SerializeField] Light[] underglow;
    [SerializeField] Light[] headlights;
    [SerializeField] Renderer glowRenderer;
    [SerializeField] [ColorUsage(false, true)] Color emissionColor = Color.cyan;
    [SerializeField] float minLight = 1f;
    [SerializeField] float maxLight = 5f;
    [SerializeField] float minEmission = 1f;
    [SerializeField] float maxEmission = 4f;
    [SerializeField] float headlightIntensity = 3f;
    [SerializeField] float pulseSpeed = 4f;
    [SerializeField] float pulseAmount = 0.08f;

    Material material;

    void Awake()
    {
        if (glowRenderer == null) return;
        material = glowRenderer.material;
        material.EnableKeyword("_EMISSION");
    }

    void Start()
    {
        foreach (var l in headlights) l.intensity = headlightIntensity;
    }

    void Update()
    {
        float t = GameManager.Instance != null ? GameManager.Instance.SpeedNormalized : 0f;
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        foreach (var l in underglow) l.intensity = Mathf.Lerp(minLight, maxLight, t) * pulse;
        if (material != null) material.SetColor("_EmissionColor", emissionColor * Mathf.Lerp(minEmission, maxEmission, t) * pulse);
    }
}
