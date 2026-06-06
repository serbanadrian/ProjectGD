using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneTransition : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;

    [Header("Player")]
    public string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Scene To Load is empty on door: " + gameObject.name);
            return;
        }

        hasTriggered = true;
        SceneManager.LoadScene(sceneToLoad);
    }
}