using UnityEngine;
using System.Collections;


public class PassingObject : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 3f;
    public float startDelay = 2f;       // Dupa cate secunde apare
    public float startX = -10f;         // Porneste din stanga
    public float endX = 10f;            // Dispare in dreapta
    public float positionY = 0f;        // Inaltimea la care trece

    [Header("Audio")]
    public AudioClip music;             // Muzica de fundal
    public float musicVolume = 1f;

    private AudioSource audioSource;
    private bool started = false;
    private bool finished = false;

    void Start()
    {
        // Ascunde obiectul la inceput
        gameObject.SetActive(false);

        // Adauga AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;

        Invoke(nameof(StartMoving), startDelay);
    }

    void StartMoving()
    {
        // Pozitioneaza la stanga
        transform.position = new Vector3(startX, positionY, 0f);
        gameObject.SetActive(true);
        started = true;

        // Porneste muzica
        if (music != null)
            audioSource.Play();
    }

    void Update()
{
    if (!started || finished) return;

    transform.position += Vector3.right * speed * Time.deltaTime;

    if (transform.position.x >= endX)
    {
        finished = true;
        StartCoroutine(FadeOutAndStop());
    }
}

IEnumerator FadeOutAndStop()
{
    float fadeDuration = 5f;  // durata fade out in secunde
    float startVolume = audioSource.volume;
    float elapsed = 0f;

    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
        yield return null;
    }

    audioSource.Stop();
    audioSource.volume = startVolume; // reseteaza volumul pentru eventuale refolosiri
    gameObject.SetActive(false);
}
}