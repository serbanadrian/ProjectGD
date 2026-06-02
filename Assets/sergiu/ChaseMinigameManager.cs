using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class ChaseMinigameManager : MonoBehaviour
{
    public static ChaseMinigameManager Instance { get; private set; }

    [Header("Chaser")]
    public GameObject chaser;

    [Header("Intro")]
    public GameObject introPanel;       // Panel cu mesajul de intro
    public TMP_Text introText;          // "Fugi de job!"
    public float introDuration = 3f;    // Cat timp sta mesajul

    [Header("Jumpscare")]
    public GameObject jumpscarePanel;
    public TMP_Text jumpscareText;
    public string nextScene = "Scene3";

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Camera Shake")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;

    private ChaseMinigame currentMinigame;
    private bool isPlaying = false;
    private bool jumpscareActive = false;
    private PlayerMovement playerMovement;
    private float survivalTime = 0f;
    private Camera mainCamera;
    private Vector3 originalCameraPos;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
        originalCameraPos = mainCamera.transform.position;
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        // Opreste playerul si chaser-ul la inceput
        if (playerMovement != null) playerMovement.enabled = false;
        if (chaser != null) chaser.SetActive(false);

        jumpscarePanel.SetActive(false);

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Arata mesajul de intro
        if (introPanel != null)
        {
            introPanel.SetActive(true);
            if (introText != null)
                introText.text = "Fugi de job!";
        }

        yield return new WaitForSeconds(introDuration);

        // Ascunde intro
        if (introPanel != null)
            introPanel.SetActive(false);

        // Porneste urmarirea
        if (playerMovement != null) playerMovement.enabled = true;
        if (chaser != null) chaser.SetActive(true);

        StartChase();
    }

    void StartChase()
    {
        if (isPlaying) return;
        isPlaying = true;

        currentMinigame = new ChaseMinigame(this);
        currentMinigame.StartMinigame(OnCaught);
    }

    void OnCaught(bool success)
    {
        if (playerMovement != null) playerMovement.enabled = false;
        StartCoroutine(CaughtSequence());
    }

    IEnumerator CaughtSequence()
    {
        // Camera shake
        yield return StartCoroutine(CameraShake());

        // Jumpscare
        jumpscarePanel.SetActive(true);
        if (jumpscareText != null)
            jumpscareText.text = "Fie că fugi de job sau după un job\n" +
                                 "răspunsul va fi același.\n\n" +
                                 "Nu da vina pe AI că ești șomer.\n\n" +
                                 "Ține minte:\n" +
                                 "\"Un om fără pantaloni nu se teme\n" +
                                 "de hoții de buzunare\"";

        jumpscareActive = true;
    }

    IEnumerator CameraShake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.position = new Vector3(
                originalCameraPos.x + offsetX,
                originalCameraPos.y + offsetY,
                originalCameraPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reseteaza camera la pozitia originala
        mainCamera.transform.position = originalCameraPos;
    }

    void Update()
    {
        if (jumpscareActive)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                jumpscareActive = false;
                jumpscarePanel.SetActive(false);

                if (GameManager.Instance != null)
                    GameManager.Instance.GoToScene(nextScene);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
            }
            return;
        }

        if (!isPlaying) return;

        survivalTime += Time.deltaTime;
        if (timerText != null)
            timerText.text = $"Supraviețuit: {survivalTime:F1}s";

        currentMinigame?.UpdateMinigame();
    }
}