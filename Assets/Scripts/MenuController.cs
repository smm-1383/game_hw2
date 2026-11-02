using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject hudRoot;
    [SerializeField] TMP_Text finalScoreText;
    [SerializeField] TMP_Text bestScoreText;

    GameState shown = (GameState)(-1);

    void Start()
    {
        Time.timeScale = 1f;
    }

    static void Show(GameObject panel, bool on)
    {
        if (panel != null) panel.SetActive(on);
    }

    static void Click()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.State == shown) return;
        shown = gm.State;
        Show(pausePanel, shown == GameState.Paused);
        Show(gameOverPanel, shown == GameState.GameOver);
        Show(hudRoot, shown == GameState.Playing || shown == GameState.Paused);
        if (shown != GameState.GameOver) return;
        if (finalScoreText != null) finalScoreText.text = "Score " + gm.Score;
        if (bestScoreText != null) bestScoreText.text = "Best " + gm.HighScore;
    }

    public void Play()
    {
        Click();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Click();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Resume()
    {
        Click();
        if (GameManager.Instance != null) GameManager.Instance.Resume();
    }

    public void Restart()
    {
        Click();
        if (GameManager.Instance != null) GameManager.Instance.Restart();
        else SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        Click();
        if (GameManager.Instance != null) GameManager.Instance.GoToMenu();
        else SceneManager.LoadScene("MainMenu");
    }
}
