using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public const float LaneWidth = 3f;
    public static GameManager Instance;

    [SerializeField] InputActionAsset controls;
    [SerializeField] int startLives = 3;
    [SerializeField] float startSpeed = 14f;
    [SerializeField] float maxSpeed = 45f;
    [SerializeField] float acceleration = 0.3f;

    InputAction pauseAction;
    int bonus;
    int coins;
    int savedBest;

    public GameState State { get; private set; }
    public int Lives { get; private set; }
    public float Distance { get; private set; }
    public float Speed { get; private set; }
    public float SpeedNormalized => Mathf.InverseLerp(startSpeed, maxSpeed, Speed);
    public float Difficulty => SpeedNormalized;
    public int Score => Mathf.FloorToInt(Distance) + bonus;
    public int Coins => coins;
    public int HighScore => Mathf.Max(savedBest, Score);

    public static float LaneX(int lane)
    {
        return (lane - 1) * LaneWidth;
    }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        State = GameState.Menu;
        Lives = startLives;
        Speed = startSpeed;
        savedBest = PlayerPrefs.GetInt("HighScore", 0);
    }

    void OnEnable()
    {
        if (controls == null) return;
        var map = controls.FindActionMap("Gameplay");
        pauseAction = map.FindAction("Pause");
        pauseAction.performed += OnPause;
        map.Enable();
    }

    void OnDisable()
    {
        if (pauseAction == null) return;
        pauseAction.performed -= OnPause;
        pauseAction.actionMap.Disable();
        pauseAction = null;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (State != GameState.Playing) return;
        Speed = Mathf.Min(maxSpeed, Speed + acceleration * Time.deltaTime);
        Distance += Speed * Time.deltaTime;
    }

    void OnPause(InputAction.CallbackContext ctx)
    {
        if (State == GameState.Playing) Pause();
        else if (State == GameState.Paused) Resume();
    }

    void SetState(GameState s)
    {
        State = s;
        Time.timeScale = s == GameState.Paused ? 0f : 1f;
    }

    public void StartGame()
    {
        Lives = startLives;
        Speed = startSpeed;
        Distance = 0f;
        bonus = 0;
        coins = 0;
        SetState(GameState.Playing);
    }

    public void Pause()
    {
        if (State == GameState.Playing) SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (State == GameState.Paused) SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (State != GameState.Playing) return;
        if (Score > savedBest)
        {
            savedBest = Score;
            PlayerPrefs.SetInt("HighScore", savedBest);
            PlayerPrefs.Save();
        }
        SetState(GameState.GameOver);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoseLife()
    {
        if (State != GameState.Playing) return;
        Lives--;
        if (Lives <= 0) GameOver();
    }

    public void AddCoin(int amount)
    {
        bonus += amount;
        coins++;
    }
}
