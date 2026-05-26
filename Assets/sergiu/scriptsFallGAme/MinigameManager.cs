using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject minigamePanel;
    public TMP_Text progressText;
    public TMP_Text livesText;
    public TMP_Text keyHintText;
    public TMP_Text feedbackText;
    public TMP_Text resultText;

    [Header("Prefabs")]
    public GameObject fallingObjectPrefab;

    [Header("Settings")]
    public MinigameType minigameType = MinigameType.Mash;

    [Header("Camera Settings")]
    public Vector3 minigameAreaCenter = new Vector3(1000f, 0f, -10f);
    public Vector3 mainScenePosition  = new Vector3(0f,    0f, -10f);

    private IMinigame currentMinigame;
    private bool isPlaying = false;
    private PlayerMovement playerMovement;
    private Camera mainCamera;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        minigamePanel.SetActive(false);
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        if (isPlaying) return;

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        isPlaying = true;

        // Muta camera la zona minigame
        mainCamera.transform.position = minigameAreaCenter;

        minigamePanel.SetActive(true);
        resultText.text  = "";
        feedbackText.text = "";
        keyHintText.text  = "";

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
        resultText.text  = success ? "SUCCES!" : "ESUAT!";
        resultText.color = success ? Color.green : Color.red;

        if (playerMovement != null) playerMovement.enabled = true;

        Invoke(nameof(ReturnToMainScene), 2f);
    }

    void ReturnToMainScene()
    {
        // Inchide panelul
        minigamePanel.SetActive(false);

        // Muta camera inapoi
        mainCamera.transform.position = mainScenePosition;
    }

    // ── UI Methods ────────────────────────────────────────────────

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
            keyHintText.text = $"Apasă [ {key} ]";
    }

    public void ShowFeedback(bool correct)
    {
        StartCoroutine(FeedbackFlash(correct));
    }

    IEnumerator FeedbackFlash(bool correct)
    {
        if (feedbackText != null)
        {
            feedbackText.text  = correct ? "✓" : "✗";
            feedbackText.color = correct ? Color.green : Color.red;
            yield return new WaitForSeconds(0.3f);
            feedbackText.text = "";
        }
    }

    public GameObject SpawnFallingObject(Vector3 position)
    {
        return Instantiate(fallingObjectPrefab, position, Quaternion.identity);
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return (this as MonoBehaviour).StartCoroutine(routine);
    }
}