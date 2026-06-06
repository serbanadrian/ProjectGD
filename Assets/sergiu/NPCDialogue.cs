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
    public string minigameScene = "Hol1";

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
         if (player == null || hasTriggered) return;

    float distance = Vector2.Distance(transform.position, player.position);
    Debug.Log($"Distanta fata de NPC: {distance}"); // ← adauga asta

    if (distance <= triggerRadius && !isRunning)
    {
        isRunning = true;
        StartCoroutine(RunDialogue());
        
    }
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

    if (dialogueBox == null) { Debug.LogError("DialogueBox e null!"); yield break; }
    if (dialogueText == null) { Debug.LogError("DialogueText e null!"); yield break; }

    dialogueBox.SetActive(true);

    // Ruleaza fiecare linie
    foreach (DialogueLine line in lines)
    {
        dialogueText.text = line.text;

        if (line.isLastLine && pressEPrompt != null)
            pressEPrompt.SetActive(true);

        yield return new WaitForSeconds(line.displayDuration);
    }

    // Asteapta E - verificat in fiecare frame
    bool ePressed = false;
    while (!ePressed)
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            ePressed = true;
        yield return null; // asteapta urmatorul frame
    }

    hasTriggered = true;
    dialogueBox.SetActive(false);
    if (pressEPrompt != null) pressEPrompt.SetActive(false);

    if (pm != null) pm.enabled = true;

    if (GameManager.Instance != null)
        GameManager.Instance.GoToScene(minigameScene);
    else
        Debug.LogError("GameManager nu exista in scena!");
}
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}