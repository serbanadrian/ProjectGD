using UnityEngine;
using UnityEngine.InputSystem;

public class ZoneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string destinationScene;     // Numele scenei destinatie
    public float interactRadius = 1.5f;
    public GameObject promptUI;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player nu are tag-ul 'Player'!");

        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactRadius;

        if (promptUI != null) promptUI.SetActive(playerInRange);

        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            GameManager.Instance.GoToScene(destinationScene);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}