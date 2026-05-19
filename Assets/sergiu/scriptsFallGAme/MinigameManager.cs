using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject minigamePanel;
    public Text progressText;       // ex. "5/10"
    public Text livesText;          // ex. "♥ ♥ ♥"
    public Text keyHintText;        // ex. "Apasă W!"
    public Text feedbackText;       // "✓" sau "✗"
    public Text resultText;

    [Header("Prefabs")]
    public GameObject fallingObjectPrefab;  // Prefab cu FallingObject.cs

    [Header("Settings")]
    public MinigameType minigameType = MinigameType.Mash;

    private IMinigame currentMinigame;
    private bool isPlaying = false;
    private PlayerMovement playerMovement;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        minigamePanel.SetActive(false);
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        if (isPlaying) return;

        isPlaying = true;
        minigamePanel.SetActive(true);
        resultText.text = "";
        feedbackText.text = "";
        keyHintText.text = "";

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
        keyHintText.text = "";
        resultText.text = success ? "SUCCES!" : "ESUAT!";
        resultText.color = success ? Color.green : Color.red;

        if (playerMovement != null) playerMovement.enabled = true;

        Invoke(nameof(ClosePanel), 2f);
    }

    void ClosePanel() => minigamePanel.SetActive(false);

    // ── Metode apelate din MashMinigame ──────────────────────────

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

    public void ShowKeyHint(string key)
    {
        if (keyHintText != null)
            keyHintText.text = $"Apasă  [ {key} ]";
    }

    public void ShowFeedback(bool correct)
    {
        StartCoroutine(FeedbackFlash(correct));
    }

    IEnumerator FeedbackFlash(bool correct)
    {
        if (feedbackText != null)
        {
            feedbackText.text = correct ? "✓" : "✗";
            feedbackText.color = correct ? Color.green : Color.red;
            yield return new WaitForSeconds(0.3f);
            feedbackText.text = "";
        }
    }

    public GameObject SpawnFallingObject(Vector3 position)
    {
        return GameObject.Instantiate(fallingObjectPrefab, position, Quaternion.identity);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return (this as MonoBehaviour).StartCoroutine(routine);
    }
}