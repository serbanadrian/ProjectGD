using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private CollectableItem currentItem;
    private ArduinoBoardManager currentBoard;
    private RobotManager currentRobot;
    private BeanBag currentBeanBag;
    private RobotDogManager currentDog;
    private bool isThreateningDog = false;
    private bool alreadyThreatenedDog = false;

    private PlayerInventory inventory;
    private Animator animator;

    public InteractionUI interactionUI;

    [Header("Threaten")]
    public float threatenDuration = 1.2f;

    private Coroutine threatenCoroutine;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        animator = GetComponent<Animator>();
    }

    public void OnInteract_arcade()
    {
        if (currentItem != null)
        {
            currentItem.Collect(inventory);
            currentItem = null;
            UpdateMessage();
            return;
        }

        if (currentBeanBag != null)
        {
            currentBeanBag.TryAskRobotForCode();
            UpdateMessage();
            return;
        }

        if (currentRobot != null)
        {
            currentRobot.Interact(inventory);
            UpdateMessage();
            return;
        }

        if (currentBoard != null)
        {
            currentBoard.InteractWithBoard(inventory);
            UpdateMessage();
            return;
        }
    }

    public void OnThreaten()
    {
        if (currentDog == null)
            return;

        if (animator == null)
            return;

        if (!isThreateningDog)
        {
            isThreateningDog = true;
            alreadyThreatenedDog = true;

            animator.SetBool("ThreateningDog", true);

            currentDog.ThreatenDog();
            UpdateMessage();
            return;
        }

        isThreateningDog = false;
        animator.SetBool("ThreateningDog", false);

        UpdateMessage();
    }

    private IEnumerator ThreatenRoutine()
    {
        animator.SetBool("ThreateningDog", true);

        yield return new WaitForSeconds(threatenDuration);

        animator.SetBool("ThreateningDog", false);
        threatenCoroutine = null;
    }

    public void OnStandSit()
    {
        if (currentBeanBag != null)
        {
            currentBeanBag.ToggleSit(animator);
            UpdateMessage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CollectableItem item = other.GetComponent<CollectableItem>();
        if (item != null)
        {
            currentItem = item;
            UpdateMessage();
            return;
        }

        RobotDogManager dog = other.GetComponent<RobotDogManager>();
        if (dog != null)
        {
            currentDog = dog;
            UpdateMessage();
            return;
        }

        BeanBag beanBag = other.GetComponent<BeanBag>();
        if (beanBag != null)
        {
            currentBeanBag = beanBag;
            UpdateMessage();
            return;
        }

        RobotManager robot = other.GetComponent<RobotManager>();
        if (robot != null)
        {
            currentRobot = robot;
            UpdateMessage();
            return;
        }

        ArduinoBoardManager board = other.GetComponent<ArduinoBoardManager>();
        if (board != null)
        {
            currentBoard = board;
            UpdateMessage();
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RobotDogManager dog = other.GetComponent<RobotDogManager>();
        if (dog != null && dog == currentDog)
        {
            currentDog = null;

            if (animator != null)
                animator.SetBool("ThreateningDog", false);

            if (threatenCoroutine != null)
            {
                StopCoroutine(threatenCoroutine);
                threatenCoroutine = null;
            }

            UpdateMessage();
            return;
        }

        BeanBag beanBag = other.GetComponent<BeanBag>();
        if (beanBag != null && beanBag == currentBeanBag)
        {
            currentBeanBag.ForceStand(animator);
            currentBeanBag = null;
            UpdateMessage();
            return;
        }

        CollectableItem item = other.GetComponent<CollectableItem>();
        if (item != null && item == currentItem)
        {
            currentItem = null;
            UpdateMessage();
            return;
        }

        RobotManager robot = other.GetComponent<RobotManager>();
        if (robot != null && robot == currentRobot)
        {
            currentRobot = null;
            UpdateMessage();
            return;
        }

        ArduinoBoardManager board = other.GetComponent<ArduinoBoardManager>();
        if (board != null && board == currentBoard)
        {
            currentBoard = null;
            UpdateMessage();
            return;
        }
    }

    private void UpdateMessage()
    {
        if (interactionUI == null)
            return;

        if (currentItem != null)
        {
            interactionUI.ShowMessage("Apasa E ca sa iei: " + currentItem.itemType);
            return;
        }

        if (currentDog != null)
        {
            interactionUI.ShowMessage(currentDog.GetInteractionMessage());
            return;
        }

        if (currentBeanBag != null)
        {
            interactionUI.ShowMessage(currentBeanBag.GetInteractionMessage());
            return;
        }

        if (currentRobot != null)
        {
            interactionUI.ShowMessage(currentRobot.GetInteractionMessage(inventory));
            return;
        }

        if (currentBoard != null)
        {
            interactionUI.ShowMessage(currentBoard.GetInteractionMessage(inventory));
            return;
        }

        interactionUI.HideMessage();
    }
}