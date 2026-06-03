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
    public TMP_Text feedbackText;
    public TMP_Text resultText;

    [Header("Falling Object Prefabs")]
    public GameObject prefabW;   // imaginea pentru W
    public GameObject prefabA;   // imaginea pentru A
    public GameObject prefabS;   // imaginea pentru S
    public GameObject prefabD;   // imaginea pentru D

    [Header("Settings")]
    public MinigameType minigameType = MinigameType.Mash;
    public string nextScene = "test";
    public int scoreOnSuccess = 100;

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
        StartMinigame(OnMinigameComplete);
    }

    void OnMinigameComplete(bool success)
    {
        if (success)
            GameManager.Instance.AddScore(scoreOnSuccess);
        else
            GameManager.Instance.LoseLife();
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        if (isPlaying) return;

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        isPlaying = true;

        minigamePanel.SetActive(true);
        resultText.text   = "";
        feedbackText.text = "";

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
        resultText.text  = success ? "SUCCES!" : "ESUAT!";
        resultText.color = success ? Color.green : Color.red;

        if (playerMovement != null) playerMovement.enabled = true;

        Invoke(nameof(GoToNextScene), 2f);
    }

    void GoToNextScene()
    {
        minigamePanel.SetActive(false);

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
        if (feedbackText != null)
        {
            feedbackText.text  = correct ? "✓" : "✗";
            feedbackText.color = correct ? Color.green : Color.red;
            yield return new WaitForSeconds(0.3f);
            feedbackText.text = "";
        }
    }

    // Spawneaza prefab-ul corespunzator tipului
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
}