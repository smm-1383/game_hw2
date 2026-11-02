using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Radio : MonoBehaviour
{
    [SerializeField] InputActionAsset controls;
    [SerializeField] AudioSource source;

    AudioClip[] clips = new AudioClip[0];
    InputAction nextAction;
    InputAction prevAction;
    InputAction muteAction;
    bool started;
    bool paused;
    float baseVolume;
    float duckUntil = -1f;

    public int Index { get; private set; }
    public int Count => clips.Length;
    public bool Muted => source != null && source.mute;
    public string CurrentTitle => clips.Length == 0 ? "" : Clean(clips[Index].name);

    static string Clean(string name)
    {
        name = Regex.Replace(name, "^[0-9]+[_\\- ]*", "");
        return name.Replace('_', ' ').Replace('-', ' ');
    }

    void Awake()
    {
        if (source != null) baseVolume = source.volume;
        clips = Resources.LoadAll<AudioClip>("Radio");
        System.Array.Sort(clips, (a, b) => string.CompareOrdinal(a.name, b.name));
    }

    void OnEnable()
    {
        if (controls == null) return;
        var map = controls.FindActionMap("Gameplay");
        nextAction = map.FindAction("NextTrack");
        prevAction = map.FindAction("PrevTrack");
        muteAction = map.FindAction("MuteRadio");
        nextAction.performed += OnNext;
        prevAction.performed += OnPrev;
        muteAction.performed += OnMute;
        map.Enable();
    }

    void OnDisable()
    {
        if (nextAction == null) return;
        nextAction.performed -= OnNext;
        prevAction.performed -= OnPrev;
        muteAction.performed -= OnMute;
        nextAction = null;
        prevAction = null;
        muteAction = null;
    }

    void OnNext(InputAction.CallbackContext ctx) { Next(); }
    void OnPrev(InputAction.CallbackContext ctx) { Prev(); }
    void OnMute(InputAction.CallbackContext ctx) { ToggleMute(); }

    public void Next() { Select(Index + 1); }
    public void Prev() { Select(Index - 1); }

    public void ToggleMute()
    {
        if (source != null) source.mute = !source.mute;
    }

    void Select(int i)
    {
        if (source == null || clips.Length == 0 || !started) return;
        Index = (i + clips.Length) % clips.Length;
        source.clip = clips[Index];
        source.Play();
        if (paused) source.Pause();
    }

    void Update()
    {
        if (source == null || clips.Length == 0) return;
        var gm = GameManager.Instance;
        var state = gm != null ? gm.State : GameState.Menu;
        if (!started)
        {
            if (state != GameState.Playing) return;
            started = true;
            Select(Random.Range(0, clips.Length));
            return;
        }
        if (state == GameState.GameOver && duckUntil < 0f)
        {
            duckUntil = Time.time + 2f;
            source.volume = baseVolume * 0.4f;
        }
        else if (state != GameState.GameOver && duckUntil >= 0f)
        {
            duckUntil = -1f;
            source.volume = baseVolume;
        }
        else if (duckUntil >= 0f && Time.time >= duckUntil && source.volume < baseVolume)
        {
            source.volume = baseVolume;
        }
        if (state == GameState.Paused && !paused)
        {
            paused = true;
            source.Pause();
        }
        else if (state != GameState.Paused && paused)
        {
            paused = false;
            source.UnPause();
        }
        if (!paused && !source.isPlaying) Next();
    }
}
