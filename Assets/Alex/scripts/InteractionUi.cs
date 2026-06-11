using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private void Awake()
    {
        HideMessage();
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
    }

    public void HideMessage()
    {
        messageText.text = "";
        messageText.gameObject.SetActive(false);
    }
}