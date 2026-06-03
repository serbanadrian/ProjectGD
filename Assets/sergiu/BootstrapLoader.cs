using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BootstrapLoader : MonoBehaviour
{
    [Header("Settings")]
    public string firstScene = "Room1";

    [Header("UI")]
    public TMP_Text pressEnterText;

    private bool canPress = false;

    void Start()
    {
        // Mica intarziere ca GameManager si SceneLoader sa se initializeze
        Invoke(nameof(ShowPrompt), 0.5f);
    }

    void ShowPrompt()
    {
        if (pressEnterText != null)
            pressEnterText.text = "Apasă ENTER pentru a începe!";
        canPress = true;
    }

    void Update()
    {
        if (!canPress) return;

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            SceneLoader.Instance.LoadScene(firstScene);
    }
}