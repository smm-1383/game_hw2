using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource engine;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioClip crashClip;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip fuelClip;
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip fireworkClip;
    [SerializeField] AudioClip gameOverClip;
    [SerializeField] float gameOverDelay = 0.4f;
    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 2f;
    [SerializeField] float minVolume = 0.3f;
    [SerializeField] float maxVolume = 0.8f;

    GameState last = (GameState)(-1);
    AudioSource[] fireworkSources;
    int nextFirework;
    float gameOverAt = -1f;

    void Awake()
    {
        Instance = this;
        fireworkSources = new AudioSource[6];
        for (int i = 0; i < fireworkSources.Length; i++)
        {
            var s = gameObject.AddComponent<AudioSource>();
            s.playOnAwake = false;
            s.spatialBlend = 0f;
            fireworkSources[i] = s;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    static void Resume(AudioSource source)
    {
        if (source == null) return;
        source.UnPause();
        if (!source.isPlaying) source.Play();
    }

    static void Pause(AudioSource source)
    {
        if (source != null) source.Pause();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        var state = gm != null ? gm.State : GameState.Menu;
        if (state != last)
        {
            last = state;
            switch (state)
            {
                case GameState.Playing:
                    Resume(music);
                    Resume(engine);
                    break;
                case GameState.Paused:
                    Pause(music);
                    Pause(engine);
                    break;
                case GameState.GameOver:
                    if (engine != null) engine.Stop();
                    gameOverAt = Time.time + (gm.Lives <= 0 ? gameOverDelay : 0f);
                    break;
                default:
                    if (gm == null) Resume(music);
                    break;
            }
        }
        if (gameOverAt >= 0f && Time.time >= gameOverAt)
        {
            gameOverAt = -1f;
            PlayGameOver();
        }
        if (state == GameState.Playing && engine != null)
        {
            engine.pitch = Mathf.Lerp(minPitch, maxPitch, gm.SpeedNormalized);
            engine.volume = Mathf.Lerp(minVolume, maxVolume, gm.SpeedNormalized);
        }
    }

    void Play(AudioClip clip)
    {
        if (sfx != null && clip != null) sfx.PlayOneShot(clip);
    }

    public void PlayCrash() { Play(crashClip); }
    public void PlayCoin() { Play(coinClip); }
    public void PlayFuel() { Play(fuelClip); }
    public void PlayClick() { Play(clickClip); }
    public void PlayGameOver() { Play(gameOverClip); }

    public void PlayFirework(float delay, float volume)
    {
        if (fireworkClip == null) return;
        var s = fireworkSources[nextFirework];
        nextFirework = (nextFirework + 1) % fireworkSources.Length;
        s.clip = fireworkClip;
        s.pitch = Random.Range(0.85f, 1.15f);
        s.volume = volume;
        s.PlayScheduled(AudioSettings.dspTime + delay);
    }
}
