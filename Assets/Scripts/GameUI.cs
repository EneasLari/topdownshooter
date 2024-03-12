using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameUI : MonoBehaviour
{

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI newWaveTitle;
    public TextMeshProUGUI newWaveEnemyCount;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI gameOverScoreUI;
    public RectTransform healthBar;


    Spawner spawner;
    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }


    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;

    }

    void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {

        float delayBeforeReverse = 2f;
        float animationSpeed = 1f;
        float animationProgress = 0;
        int animationDirection = 1;

        float endDelayTime = Time.time + 1 / animationSpeed + delayBeforeReverse;
        newWaveBanner.gameObject.SetActive(true);
        while (animationProgress >= 0)
        {
            animationProgress += Time.deltaTime * animationSpeed * animationDirection;

            if (animationProgress >= 1)
            {
                animationProgress = 1;
                if (Time.time > endDelayTime)
                {
                    animationDirection = -1;
                }
            }

            float canvasHeight = GetComponent<Canvas>().GetComponent<RectTransform>().sizeDelta.y;
            float bannerHeight = newWaveBanner.sizeDelta.y;
            float offScreenY = (canvasHeight / 2) + (bannerHeight / 2);
            float onScreenY = (canvasHeight / 2) - (bannerHeight / 2);

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-offScreenY, -(onScreenY - (bannerHeight / 2)), animationProgress);
            yield return null;
        }
        newWaveBanner.gameObject.SetActive(false);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}