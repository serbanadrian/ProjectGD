using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class InteractableObject : MonoBehaviour
{
    [Header("Settings")]
    public float interactRadius = 2f;

    [Header("UI References")]
    public GameObject promptUI;   // Un Text/Panel din Canvas cu "Apasa [E]"

    private Transform player;
    private bool playerInRange = false;
    private bool hasInteracted = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (hasInteracted)
        {
            if (promptUI != null) promptUI.SetActive(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactRadius;

        // Arata/ascunde prompt
        if (promptUI != null) promptUI.SetActive(playerInRange);

        // Interactiune
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            MinigameManager.Instance.StartMinigame(OnMinigameComplete);
        }
    }

    void OnMinigameComplete(bool success)
    {
        hasInteracted = true;
        Debug.Log(success ? "Minigame reusit!" : "Minigame esuat!");
        // Logica ta aici (reward, animatie, etc.)
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}