using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueLine[] lines;
    public float triggerRadius = 3f;

    [Header("UI References")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public GameObject pressEPrompt;

    [Header("Next Scene")]
    public string minigameScene = "Hol1";

    private Transform player;
    private bool hasTriggered = false;
    private bool isRunning = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player nu are tag-ul 'Player'!");
        }

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);
    }

    void Update()
    {
        if (player == null || hasTriggered || isRunning)
            return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log($"Distanta fata de NPC: {distance}");

        if (distance <= triggerRadius)
        {
            isRunning = true;
            StartCoroutine(RunDialogue());
        }
    }

    IEnumerator RunDialogue()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();

        if (pm != null)
            pm.enabled = false;

        if (dialogueBox == null)
        {
            Debug.LogError("DialogueBox e null!");
            yield break;
        }

        if (dialogueText == null)
        {
            Debug.LogError("DialogueText e null!");
            yield break;
        }

        dialogueBox.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            dialogueText.text = line.text;

            if (line.isLastLine && pressEPrompt != null)
                pressEPrompt.SetActive(true);

            yield return new WaitForSeconds(line.displayDuration);
        }

        while (!Keyboard.current.eKey.wasPressedThisFrame)
        {
            yield return null;
        }

        hasTriggered = true;

        dialogueBox.SetActive(false);

        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);

        if (pm != null)
            pm.enabled = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToScene(minigameScene);
        }
        else
        {
            Debug.LogError("GameManager nu exista in scena!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}