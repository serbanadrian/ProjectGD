using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("Settings")]
    public MinigameType minigameType = MinigameType.Mash;
    public string nextScene = "Scene2";
    public int scoreOnSuccess = 100;

    private IMinigame currentMinigame;
    private bool isPlaying = false;
    private PlayerMovement playerMovement;

    [Header("UI References")]
    public GameObject minigamePanel;
    public TMP_Text progressText;
    public TMP_Text livesText;
    public TMP_Text feedbackText;
    public TMP_Text resultText;
    public TMP_Text timerText;

    [Header("Falling Object Prefabs")]
    public GameObject prefabW;
    public GameObject prefabA;
    public GameObject prefabS;
    public GameObject prefabD;

    [Header("Chase Settings")]
    public GameObject chaser;

    [Header("Chase UI")]
    public GameObject introPanel;
    public TMP_Text introText;
    public GameObject jumpscarePanel;
    public TMP_Text jumpscareText;

    [Header("StayAwake Settings")]
    public Slider sleepSlider;
    public Image sleepSliderFill;
    public Image sleepOverlay;
    public TMP_Text currentKeyText;
    public TMP_Text stayAwakeTimerText;
    public TMP_Text resultStayAwakeText;
    public TMP_Text feedbackStayAwakeText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        // ← null checks peste tot
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (introPanel != null)    introPanel.SetActive(false);
        if (jumpscarePanel != null) jumpscarePanel.SetActive(false);
        if (sleepOverlay != null)  sleepOverlay.color = new Color(0f, 0f, 0f, 0f);

        StartMinigame(OnMinigameComplete);
    }

    void OnMinigameComplete(bool success)
    {
        if (GameManager.Instance != null)
        {
            if (success) GameManager.Instance.AddScore(scoreOnSuccess);
            else GameManager.Instance.LoseLife();
        }
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        if (isPlaying) return;

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        isPlaying = true;

        if (minigamePanel != null)  minigamePanel.SetActive(true);
        if (resultText != null)     resultText.text    = "";
        if (feedbackText != null)   feedbackText.text  = "";
        if (currentKeyText != null) currentKeyText.text = "";
        if (timerText != null)      timerText.text     = "";

        if (playerMovement != null) playerMovement.enabled = false;

        currentMinigame = MinigameFactory.Create(minigameType, this);
        currentMinigame.StartMinigame((success) =>
        {
            OnMinigameFinished(success);
            onComplete?.Invoke(success);
        });
    }

    void Update()
    {
        if (!isPlaying) return;
        currentMinigame?.UpdateMinigame();
    }

   void OnMinigameFinished(bool success)
{
    isPlaying = false;

    if (currentKeyText != null) currentKeyText.text = "";
    if (stayAwakeTimerText != null) stayAwakeTimerText.text = "";

    TMP_Text targetResult = minigameType == MinigameType.StayAwake
        ? resultStayAwakeText
        : resultText;

    if (targetResult != null)
    {
        targetResult.text  = success ? "SUCCES!" : "ESUAT!";
        targetResult.color = success ? Color.green : Color.red;
    }

    if (playerMovement != null) playerMovement.enabled = true;

    Invoke(nameof(GoToNextScene), 2f);
}

    void GoToNextScene()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.GoToScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    // ── UI Methods ────────────────────────────────────────

    public void UpdateProgress(int current, int total)
    {
        if (progressText != null)
            progressText.text = $"{current}/{total}";
    }

    public void UpdateLives(int current, int max)
    {
        if (livesText != null)
        {
            string hearts = "";
            for (int i = 0; i < max; i++)
                hearts += i < current ? "♥ " : "♡ ";
            livesText.text = hearts.Trim();
        }
    }

    public void ShowFeedback(bool correct)
{
    StartCoroutine(FeedbackFlash(correct));
}

IEnumerator FeedbackFlash(bool correct)
{
    TMP_Text target = minigameType == MinigameType.StayAwake 
        ? feedbackStayAwakeText 
        : feedbackText;

    if (target != null)
    {
        target.text  = correct ? "✓" : "✗";
        target.color = correct ? Color.green : Color.red;
        yield return new WaitForSeconds(0.3f);
        target.text = "";
    }
}


    public GameObject SpawnFallingObject(FallingObject.ObjectType type, Vector3 position)
    {
        GameObject prefab = type switch
        {
            FallingObject.ObjectType.W => prefabW,
            FallingObject.ObjectType.A => prefabA,
            FallingObject.ObjectType.S => prefabS,
            FallingObject.ObjectType.D => prefabD,
            _ => prefabW
        };

        if (prefab == null)
        {
            Debug.LogError($"Prefab pentru {type} nu e asignat in Inspector!");
            return null;
        }

        return Instantiate(prefab, position, Quaternion.identity);
    }

    public void SetMinigameType(MinigameType type) => minigameType = type;

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return (this as MonoBehaviour).StartCoroutine(routine);
    }

    public void UpdateSleepBar(float value)
    {
        if (sleepSlider != null)
            sleepSlider.value = value;

        if (sleepSliderFill != null)
            sleepSliderFill.color = Color.Lerp(Color.green, Color.red, value);
    }

    public void UpdateOverlay(float value)
    {
        if (sleepOverlay != null)
            sleepOverlay.color = new Color(0f, 0f, 0f, value * 0.85f);
    }

    public void ShowCurrentKey(string key)
    {
        if (currentKeyText != null)
            currentKeyText.text = $"Apasă [ {key} ]";
    }

    public void ShowIntro(string message, float duration)
    {
        StartCoroutine(IntroCoroutine(message, duration));
    }

    IEnumerator IntroCoroutine(string message, float duration)
    {
        if (introPanel != null && introText != null)
        {
            introPanel.SetActive(true);
            introText.text = message;
            yield return new WaitForSeconds(duration);
            introPanel.SetActive(false);
        }
    }

    public void ShowJumpscare(string message)
    {
        if (jumpscarePanel != null)
        {
            jumpscarePanel.SetActive(true);
            if (jumpscareText != null)
                jumpscareText.text = message;
        }
    }

  public void UpdateTimer(float timeRemaining)
{
    if (minigameType == MinigameType.StayAwake)
    {
        if (stayAwakeTimerText != null)
            stayAwakeTimerText.text = $"Timp: {timeRemaining:F0}s";
    }
    else
    {
        if (timerText != null)
            timerText.text = $"Timp: {timeRemaining:F0}s";
    }
}
}