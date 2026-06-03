using UnityEngine;

public class ChairSitTrigger : MonoBehaviour
{
    [Header("Cheating Game Manager")]
    public CheatingGameManager cheatingGameManager;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        if (cheatingGameManager != null)
        {
            cheatingGameManager.StartSitting();
        }
    }
}