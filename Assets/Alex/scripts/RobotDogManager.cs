using UnityEngine;

public class RobotDogManager : MonoBehaviour
{
    [Header("References")]
    public RobotManager robotManager;
    public InteractionUI interactionUI;

    [Header("Messages")]
    public string threatenMessage = "Apasa R ca sa ameninti catelul robot";
    public string successMessage = "Robotul este acum willing to help";

    private bool playerInside = false;
    private bool alreadyThreatened = false;

    private void Awake()
    {
        if (robotManager == null)
        {
            robotManager = FindFirstObjectByType<RobotManager>();
        }
    }

    public string GetInteractionMessage()
    {
        if (alreadyThreatened)
        {
            return "Catelul robot a fost deja amenintat";
        }

        return threatenMessage;
    }

    public void ThreatenDog()
    {
        Debug.Log("ThreatenDog apelat");

        if (!playerInside)
        {
            Debug.Log("Playerul nu este in trigger-ul cainelui");
            return;
        }

        if (alreadyThreatened)
        {
            Debug.Log("Cainele a fost deja amenintat");
            return;
        }

        alreadyThreatened = true;

        if (robotManager != null)
        {
            robotManager.MakeWillingToHelp();
            Debug.Log("Robot willingToHelp = true");
        }
        else
        {
            Debug.LogWarning("RobotManager nu este conectat la caine");
        }

        if (interactionUI != null)
        {
            interactionUI.ShowMessage(successMessage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DOG TRIGGER ENTER: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (interactionUI != null)
            {
                interactionUI.ShowMessage(GetInteractionMessage());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("DOG TRIGGER EXIT: " + other.name);

        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (interactionUI != null)
            {
                interactionUI.HideMessage();
            }
        }
    }
}