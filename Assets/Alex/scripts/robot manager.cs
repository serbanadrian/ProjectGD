using System.Collections;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [Header("Movement")]
    public float wakeUpDuration = 1.5f;
    public float walkSouthTime = 0.76f;
    public float walkEastTime = 1.5f;
    public float moveSpeed = 2f;

    [Header("Animator Parameters")]
    public string activatedParameter = "isActivated";
    public string helpBuildCodeParameter = "HelpBuildcode";

    [Header("Animator States")]
    public string walkSouthAnim = "walk_south";
    public string walkEastAnim = "walk east";
    public string idleEastAnim = "east_idle";

    [Header("Code Writing")]
    public float helpBuildCodeDuration = 2f;
    public bool codeAlreadyWritten = false;

    [Header("Robot State")]
    public bool willingToHelp = false;
    public bool isAtLaptop = false;

    [Header("References")]
    public ArduinoBoardManager boardManager;

    private Animator animator;
    private bool isAwake = false;
    private bool isMoving = false;
    private bool isWritingCode = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool CanWakeUp(PlayerInventory inventory)
    {
        return !isAwake && !isMoving && inventory.HasItem(ItemType.Screwdriver);
    }

    public string GetInteractionMessage(PlayerInventory inventory)
    {
        if (CanWakeUp(inventory))
            return "Apasa E ca sa activezi robotul cu surubelnita";

        if (isWritingCode)
            return "Robotul scrie codul...";

        if (isAtLaptop && willingToHelp && !codeAlreadyWritten)
            return "Robotul e pregatit sa scrie codul";

        if (isAtLaptop && willingToHelp && codeAlreadyWritten)
            return "Codul a fost deja scris";

        if (isAtLaptop && !willingToHelp)
            return "Robotul e la laptop, dar nu vrea sa ajute";

        if (isAwake)
            return "Robotul merge spre laptop";

        return "Ai nevoie de surubelnita";
    }

    public void Interact(PlayerInventory inventory)
    {
        if (!CanWakeUp(inventory))
            return;

        StartCoroutine(WakeUpAndMoveToLaptop());
    }

    private IEnumerator WakeUpAndMoveToLaptop()
    {
        isMoving = true;
        isAwake = true;

        if (animator != null)
            animator.SetBool(activatedParameter, true);

        yield return new WaitForSeconds(wakeUpDuration);

        if (animator != null)
            animator.Play(walkSouthAnim);

        yield return MoveForTime(Vector2.down, walkSouthTime);

        if (animator != null)
            animator.Play(walkEastAnim);

        yield return MoveForTime(Vector2.right, walkEastTime);

        if (animator != null)
            animator.Play(idleEastAnim);

        isAtLaptop = true;
        isMoving = false;
    }

    private IEnumerator MoveForTime(Vector2 direction, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void MakeWillingToHelp()
    {
        willingToHelp = true;
    }

    public void HelpBuildCode()
    {
        if (!isAtLaptop || !willingToHelp)
            return;

        if (isWritingCode || codeAlreadyWritten)
            return;

        StartCoroutine(HelpBuildCodeRoutine());
    }

    private IEnumerator HelpBuildCodeRoutine()
    {
        isWritingCode = true;
        codeAlreadyWritten = true;

        if (animator != null)
        {
            animator.SetBool(helpBuildCodeParameter, true);
        }

        yield return new WaitForSeconds(helpBuildCodeDuration);

        if (boardManager != null)
        {
            boardManager.ReceiveCodeFromRobot(1);
        }
        else
        {
            Debug.LogWarning("RobotManager nu are BoardManager conectat.");
        }

        if (animator != null)
        {
            animator.SetBool(helpBuildCodeParameter, false);
            animator.Play(idleEastAnim);
        }

        isWritingCode = false;
    }

    public bool CanWriteCode()
    {
        return isAtLaptop && willingToHelp && !isWritingCode && !codeAlreadyWritten;
    }
}