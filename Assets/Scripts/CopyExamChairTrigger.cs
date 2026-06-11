using UnityEngine;

public class CopyExamChairTrigger : MonoBehaviour
{
    [Header("Manager")]
    public CopyExamManager copyExamManager;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        if (!other.CompareTag("Player"))
            return;

        used = true;

        if (copyExamManager != null)
        {
            copyExamManager.SitDown();
        }
        else
        {
            Debug.LogWarning("CopyExamManager is missing on " + gameObject.name);
        }
    }
}