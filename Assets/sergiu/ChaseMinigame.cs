using UnityEngine;
using System;
using System.Collections;

public class ChaseMinigame : IMinigame
{
    public static ChaseMinigame Instance { get; private set; }

    private MinigameManager manager;
    private Action<bool> onComplete;

    private Transform chaser;
    private Transform player;

    private float chaseSpeed = 3f;
    private float catchRadius = 0.5f;
    private float introDuration = 3f;

    private bool isRunning = false;
    private bool caught = false;
    private bool introFinished = false;

    public ChaseMinigame(MinigameManager manager)
    {
        this.manager = manager;
        Instance = this;
    }

    public void StartMinigame(Action<bool> onComplete)
    {
        this.onComplete = onComplete;
        isRunning = false;  // nu porneste pana nu se termina intro-ul
        caught = false;
        introFinished = false;

        chaser = manager.chaser.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Opreste playerul la inceput
        var pm = GameObject.FindFirstObjectByType<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Ascunde chaser-ul la inceput
        manager.chaser.SetActive(false);

        // Porneste intro
        manager.StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
{
    manager.ShowIntro("Fugi de job!", introDuration);
    yield return new WaitForSeconds(introDuration);

    manager.chaser.SetActive(true);

    var pm = GameObject.FindFirstObjectByType<PlayerMovement>();
    if (pm != null) pm.enabled = true;

    // ← porneste muzica de urmarire
    manager.PlayChaseMusic();

    introFinished = true;
    isRunning = true;
}

    public void UpdateMinigame()
    {
        if (!isRunning || caught || !introFinished) return;

        // Urmareste playerul
        Vector3 direction = (player.position - chaser.position).normalized;
        chaser.position += direction * chaseSpeed * Time.deltaTime;

        // Verifica distanta
        float distance = Vector2.Distance(chaser.position, player.position);
        if (distance <= catchRadius)
        {
            caught = true;
            isRunning = false;

            // Opreste playerul
            var pm = GameObject.FindFirstObjectByType<PlayerMovement>();
            if (pm != null) pm.enabled = false;

            // Camera shake + jumpscare
            manager.StartCoroutine(CaughtSequence());
        }
    }

   IEnumerator CaughtSequence()
{
    // ← opreste muzica de urmarire, porneste sunetul de prins
    manager.PlayCaughtSound();

    yield return manager.StartCoroutine(CameraShake(0.5f, 0.3f));

    manager.ShowJumpscare(
        "Fie că fugi de job sau după un job\n" +
        "răspunsul va fi același.\n\n" +
        "Nu da vina pe AI că ești șomer.\n\n" +
        "Ține minte:\n" +
        "\"Un om fără pantaloni nu se teme\n" +
        "de hoții de buzunare\""
    );

    bool pressed = false;
    while (!pressed)
    {
        if (UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
            pressed = true;
        yield return null;
    }

    EndMinigame(false);
}


    IEnumerator CameraShake(float duration, float magnitude)
    {
        Camera cam = Camera.main;
        Vector3 originalPos = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            cam.transform.position = new Vector3(
                originalPos.x + offsetX,
                originalPos.y + offsetY,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = originalPos;
    }

    public void EndMinigame(bool success)
    {
        isRunning = false;
        Instance = null;

        // Ascunde jumpscare
        if (manager.jumpscarePanel != null)
            manager.jumpscarePanel.SetActive(false);

        onComplete?.Invoke(false);
    }
}