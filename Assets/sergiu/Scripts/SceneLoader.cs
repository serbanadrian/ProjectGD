using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public CanvasGroup fadeCanvasGroup; // Canvas negru care face fade

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade in (negru apare)
        yield return StartCoroutine(Fade(0f, 1f));

        // Incarca scena
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => op.isDone);

        // Fade out (negru dispare)
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;
        fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
        fadeCanvasGroup.blocksRaycasts = (to > 0f);
    }
}