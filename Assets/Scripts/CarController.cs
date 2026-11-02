using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] InputActionAsset controls;
    [SerializeField] Transform body;
    [SerializeField] Renderer bodyRenderer;
    [SerializeField] ParticleSystem sparks;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] Fuel fuel;
    [SerializeField] float smoothTime = 0.12f;
    [SerializeField] float maxTilt = 7.5f;
    [SerializeField] float invincibleTime = 1.5f;
    [SerializeField] Color flashColor = Color.red;

    InputAction steerAction;
    Material material;
    Color baseColor;
    Color emission;
    bool hasEmission;
    bool flashing;
    float velocityX;
    float tilt;
    float tiltVelocity;
    float invincibleUntil;

    public int LaneIndex { get; private set; } = 1;
    public Fuel Fuel => fuel;

    void Awake()
    {
        if (fuel == null) fuel = GetComponent<Fuel>();
        if (bodyRenderer == null) return;
        material = bodyRenderer.material;
        if (material.HasProperty(BaseColorId)) baseColor = material.GetColor(BaseColorId);
        hasEmission = material.HasProperty(EmissionId);
        if (hasEmission) emission = material.GetColor(EmissionId);
    }

    void OnEnable()
    {
        if (controls == null) return;
        var map = controls.FindActionMap("Gameplay");
        steerAction = map.FindAction("Steer");
        steerAction.performed += OnSteer;
        map.Enable();
    }

    void OnDisable()
    {
        if (steerAction == null) return;
        steerAction.performed -= OnSteer;
        steerAction.actionMap.Disable();
        steerAction = null;
    }

    void OnSteer(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
        float value = ctx.ReadValue<float>();
        if (value > 0.5f) LaneIndex = Mathf.Min(2, LaneIndex + 1);
        else if (value < -0.5f) LaneIndex = Mathf.Max(0, LaneIndex - 1);
    }

    void Update()
    {
        var pos = transform.position;
        pos.x = Mathf.SmoothDamp(pos.x, GameManager.LaneX(LaneIndex), ref velocityX, smoothTime);
        transform.position = pos;

        if (body != null)
        {
            float target = Mathf.Clamp(velocityX, -20f, 20f) / 20f * maxTilt;
            tilt = Mathf.SmoothDamp(tilt, target, ref tiltVelocity, smoothTime);
            body.localRotation = Quaternion.Euler(0f, tilt, -tilt);
        }

        bool shouldFlash = Time.time < invincibleUntil && Mathf.PingPong(Time.time * 6f, 1f) > 0.5f;
        if (shouldFlash != flashing) SetFlash(shouldFlash);
    }

    void SetFlash(bool on)
    {
        flashing = on;
        if (material == null) return;
        if (baseColor.a > 0f) material.SetColor(BaseColorId, on ? flashColor : baseColor);
        if (!hasEmission) return;
        if (on) material.EnableKeyword("_EMISSION");
        material.SetColor(EmissionId, on ? flashColor * 2f : emission);
    }

    public void Crash()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Playing || Time.time < invincibleUntil) return;
        invincibleUntil = Time.time + invincibleTime;
        gm.LoseLife();
        if (sparks != null) sparks.Play();
        if (cameraShake != null) cameraShake.Shake(0.4f, 0.4f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCrash();
    }

    void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponentInParent<Interactable>();
        if (item != null) item.OnHit(this);
    }
}
