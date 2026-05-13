using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.InputSystem;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("UI References")]
    public GameObject minigamePanel;       // Panelul principal
    public Image timerBar;                 // Image cu Fill Method = Horizontal
    public Image markerImage;              // Indicatorul care se misca
    public Image successZoneImage;         // Zona verde de succes
    public Text instructionText;           // "Apasa SPACE!"

    [Header("Minigame Settings")]
    public float markerSpeed = 1.5f;       // Viteza markerului
    public float successZoneWidth = 0.2f;  // Cat de larga e zona de succes (0-1)
    public float successZonePosition = 0.5f; // Pozitia centrului zonei de succes (0-1)
    public int requiredPresses = 3;        // Cate apasari corecte trebuie

    private bool isPlaying = false;
    private float markerPosition = 0f;
    private int markerDirection = 1;
    private int correctPresses = 0;
    private Action<bool> onComplete;

    void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
    }

    public void StartMinigame(Action<bool> callback)
    {
        if (isPlaying) return;

        onComplete = callback;
        correctPresses = 0;
        markerPosition = 0f;
        markerDirection = 1;
        isPlaying = true;

        // Pozitioneaza zona de succes
        if (successZoneImage != null)
        {
            RectTransform zoneRect = successZoneImage.GetComponent<RectTransform>();
            zoneRect.anchorMin = new Vector2(successZonePosition - successZoneWidth / 2f, 0);
            zoneRect.anchorMax = new Vector2(successZonePosition + successZoneWidth / 2f, 1);
            zoneRect.offsetMin = Vector2.zero;
            zoneRect.offsetMax = Vector2.zero;
        }

        minigamePanel.SetActive(true);
        UpdateInstructionText();

        // Opreste miscarea playerului
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) pm.enabled = false;
    }

    void Update()
    {
        if (!isPlaying) return;

        // Misca markerul dus-intors
        markerPosition += markerDirection * markerSpeed * Time.deltaTime;

        if (markerPosition >= 1f) { markerPosition = 1f; markerDirection = -1; }
        if (markerPosition <= 0f) { markerPosition = 0f; markerDirection = 1; }

        // Actualizeaza pozitia markerului pe UI
        if (markerImage != null)
        {
            RectTransform markerRect = markerImage.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(markerPosition - 0.01f, 0);
            markerRect.anchorMax = new Vector2(markerPosition + 0.01f, 1);
            markerRect.offsetMin = Vector2.zero;
            markerRect.offsetMax = Vector2.zero;
        }

        // Input
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CheckPress();
        }

        // Escape = abandon
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndMinigame(false);
        }
    }

    void CheckPress()
    {
        float zoneMin = successZonePosition - successZoneWidth / 2f;
        float zoneMax = successZonePosition + successZoneWidth / 2f;

        bool inZone = markerPosition >= zoneMin && markerPosition <= zoneMax;

        if (inZone)
        {
            correctPresses++;
            UpdateInstructionText();

            if (correctPresses >= requiredPresses)
            {
                EndMinigame(true);
            }
        }
        else
        {
            // Gresit — resetam progresul sau scadem
            correctPresses = Mathf.Max(0, correctPresses - 1);
            UpdateInstructionText();
            StartCoroutine(FlashFeedback(false));
        }
    }

    void UpdateInstructionText()
    {
        if (instructionText != null)
            instructionText.text = $"Apasă SPACE în zona verde! ({correctPresses}/{requiredPresses})";
    }

    IEnumerator FlashFeedback(bool success)
    {
        if (markerImage != null)
        {
            markerImage.color = success ? Color.green : Color.red;
            yield return new WaitForSeconds(0.15f);
            markerImage.color = Color.white;
        }
    }

    void EndMinigame(bool success)
    {
        isPlaying = false;
        minigamePanel.SetActive(false);

        // Reactivam miscarea playerului
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        onComplete?.Invoke(success);
    }
}