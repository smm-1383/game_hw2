using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] Image fuelFill;
    [SerializeField] Image[] lifeIcons;
    [SerializeField] TMP_Text radioText;
    [SerializeField] Radio radio;
    [SerializeField] Fuel fuel;
    [SerializeField] Color fuelColor = Color.green;
    [SerializeField] Color fuelLowColor = Color.red;
    [SerializeField] Color lostLifeColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] Color coinFlashColor = new Color(1f, 0.8f, 0.1f);
    [SerializeField] float coinFlashTime = 0.3f;
    [SerializeField] float coinFlashScale = 0.25f;

    Color scoreColor = Color.white;
    int lastCoins;
    float flash;
    string lastTitle;
    bool lastMuted;

    void Start()
    {
        if (scoreText != null) scoreColor = scoreText.color;
    }

    void Update()
    {
        if (radioText != null && radio != null && radio.Count > 0)
        {
            var title = radio.CurrentTitle;
            if (title != lastTitle || radio.Muted != lastMuted)
            {
                lastTitle = title;
                lastMuted = radio.Muted;
                if (title.Length > 32) title = title.Substring(0, 32) + "...";
                radioText.text = "<size=55%>RADIO   Q / E change   M mute</size>\n" + (radio.Muted ? "Muted" : title);
            }
        }
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (speedText != null) speedText.text = Mathf.RoundToInt(gm.Speed * 3.6f) + " km/h";
        if (scoreText != null) scoreText.text = "Score " + gm.Score;
        if (highScoreText != null) highScoreText.text = "Best " + gm.HighScore;
        if (coinsText != null) coinsText.text = "Coins " + gm.Coins;

        if (gm.Coins > lastCoins) flash = coinFlashTime;
        lastCoins = gm.Coins;
        if (scoreText != null)
        {
            flash = Mathf.Max(0f, flash - Time.unscaledDeltaTime);
            float k = flash / coinFlashTime;
            scoreText.color = Color.Lerp(scoreColor, coinFlashColor, k);
            scoreText.transform.localScale = Vector3.one * (1f + coinFlashScale * k);
        }

        if (fuelFill != null && fuel != null)
        {
            fuelFill.fillAmount = fuel.Normalized;
            fuelFill.color = fuel.Normalized < 0.25f ? fuelLowColor : fuelColor;
        }

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].color = i < gm.Lives ? Color.white : lostLifeColor;
        }
    }
}
