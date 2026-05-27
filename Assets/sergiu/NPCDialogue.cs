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
    public GameObject dialogueBox;      // Panelul casutei de dialog
    public TMP_Text dialogueText;       // Textul din casuta
    public GameObject pressEPrompt;     // "Apasa E pentru a incepe"

    [Header("Next Scene")]
    public string minigameScene = "Minigame1";

    private Transform player;
    private bool hasTriggered = false;
    private bool isRunning = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player nu are tag-ul 'Player'!");

        if (dialogueBox != null)    dialogueBox.SetActive(false);
        if (pressEPrompt != null)   pressEPrompt.SetActive(false);
    }

    void Update()
    {
        if (player == null || hasTriggered) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= triggerRadius && !isRunning)
        {
            isRunning = true;
            StartCoroutine(RunDialogue());
        }
    }

    IEnumerator RunDialogue()
    {
        // Opreste playerul
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        dialogueBox.SetActive(true);

        // Ruleaza fiecare linie
        foreach (DialogueLine line in lines)
        {
            dialogueText.text = line.text;

            // Daca e ultima linie, arata promptul de E
            if (line.isLastLine && pressEPrompt != null)
                pressEPrompt.SetActive(true);

            yield return new WaitForSeconds(line.displayDuration);
        }

        // Asteapta E dupa ultima linie
        yield return new WaitUntil(() =>
            Keyboard.current.eKey.wasPressedThisFrame);

        hasTriggered = true;
        dialogueBox.SetActive(false);
        if (pressEPrompt != null) pressEPrompt.SetActive(false);

        // Reactiveaza playerul si merge la minigame
        if (pm != null) pm.enabled = true;
        GameManager.Instance.GoToScene(minigameScene);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}