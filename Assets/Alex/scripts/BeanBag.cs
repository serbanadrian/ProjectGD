using UnityEngine;

public class BeanBag : MonoBehaviour
{
    [Header("References")]
    public RobotManager robotManager;
    public InteractionUI interactionUI;

    [Header("Player Animator")]
    public string sittingParameter = "isSitting";

    private bool playerInside = false;
    private bool playerIsSitting = false;

    public string GetInteractionMessage()
    {
        if (!playerInside)
            return "";

        if (!playerIsSitting)
            return "Click dreapta ca sa te asezi";

        if (robotManager == null)
            return "Robotul nu este conectat la beanbag";

        if (!robotManager.isAtLaptop)
            return "Robotul trebuie sa fie la laptop";

        if (!robotManager.willingToHelp)
            return "Don't know how to write code";

        if (robotManager.codeReadyToCollect)
            return "Codul este gata. Mergi la robot si apasa E";

        return "Robot is willing to help. Apasa E ca sa scrie codul";
    }

    public void ToggleSit(Animator playerAnimator)
    {
        if (!playerInside)
            return;

        playerIsSitting = !playerIsSitting;

        if (playerAnimator != null)
            playerAnimator.SetBool(sittingParameter, playerIsSitting);

        if (interactionUI != null)
            interactionUI.ShowMessage(GetInteractionMessage());
    }

    public void ForceStand(Animator playerAnimator)
    {
        playerIsSitting = false;

        if (playerAnimator != null)
            playerAnimator.SetBool(sittingParameter, false);
    }

    public void TryAskRobotForCode()
    {
        if (!playerInside || !playerIsSitting)
            return;

        if (robotManager == null)
        {
            interactionUI.ShowMessage("Robotul nu este conectat");
            return;
        }

        if (!robotManager.isAtLaptop)
        {
            interactionUI.ShowMessage("Robotul trebuie sa fie la laptop");
            return;
        }

        if (!robotManager.willingToHelp)
        {
            interactionUI.ShowMessage("Don't know how to write code");
            return;
        }

        robotManager.HelpBuildCode();

        interactionUI.ShowMessage("Robotul scrie codul...");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerInventory>() != null)
        {
            playerInside = true;

            if (interactionUI != null)
                interactionUI.ShowMessage(GetInteractionMessage());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerInventory>() != null)
        {
            playerInside = false;
            playerIsSitting = false;

            if (interactionUI != null)
                interactionUI.HideMessage();
        }
    }
}