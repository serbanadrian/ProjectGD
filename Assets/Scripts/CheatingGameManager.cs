using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CheatingGameManager : MonoBehaviour
{
    public enum CheatingGameState
    {
        FreeRoam,
        Sitting,
        IdleSeated,
        Copying,
        Finished,
        Caught
    }

    [Header("Current State")]
    public CheatingGameState currentState = CheatingGameState.FreeRoam;

    [Header("Objects Before Sitting")]
    public GameObject player;
    public GameObject chair;
    public GameObject deskOff;

    [Header("Objects After Sitting")]
    public GameObject seatedStudent;
    public GameObject deskOn;

    [Header("Professor")]
    public ProfessorPatrol professorPatrol;

    [Header("Sorting Order")]
    public int seatedStudentSortingOrder = 6;
    public int deskOnSortingOrder = 5;

    [Header("Grade UI")]
    public TextMeshProUGUI gradeText;

    [Header("Try Again UI")]
    public GameObject tryAgainButton;

    [Header("Grade Settings")]
    public float grade = 0f;
    public float maxGrade = 10f;

    [Tooltip("La câte secunde crește nota cu 0.01")]
    public float incrementInterval = 0.05f;

    [Header("Animations")]
    public Animator seatedStudentAnimator;
    public Animator pcAnimator;

    [Header("Animation Parameters")]
    public string copyingBoolName = "isCopying";
    public string caughtTriggerName = "getCaught";
    public string caughtFinalBoolName = "isCaughtFinal";

    [Header("Scene")]
    public bool loadNextSceneWhenDone = true;

    private float timer = 0f;
    private bool caughtSequenceStarted = false;

    private void Start()
    {
        UpdateGradeText();
        SetCopyingAnimation(false);
        SetCaughtFinalAnimation(false);

        if (tryAgainButton != null)
            tryAgainButton.SetActive(false);

        ChangeState(CheatingGameState.FreeRoam);
    }

    private void Update()
    {
        switch (currentState)
        {
            case CheatingGameState.FreeRoam:
                HandleFreeRoamState();
                break;

            case CheatingGameState.Sitting:
                HandleSittingState();
                break;

            case CheatingGameState.IdleSeated:
                HandleIdleSeatedState();
                break;

            case CheatingGameState.Copying:
                HandleCopyingState();
                break;

            case CheatingGameState.Finished:
                HandleFinishedState();
                break;

            case CheatingGameState.Caught:
                HandleCaughtState();
                break;
        }
    }

    private void HandleFreeRoamState()
    {
        // Playerul se mișcă normal.
    }

    private void HandleSittingState()
    {
        SitDown();
        ChangeState(CheatingGameState.IdleSeated);
    }

    private void HandleIdleSeatedState()
    {
        SetCopyingAnimation(false);
        timer = 0f;

        bool wantsToCopy = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

        if (wantsToCopy)
        {
            if (ProfessorCanCatch())
            {
                StartCaughtSequence();
                return;
            }

            ChangeState(CheatingGameState.Copying);
        }
    }

    private void HandleCopyingState()
    {
        bool isStillCopying = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

        if (ProfessorCanCatch())
        {
            StartCaughtSequence();
            return;
        }

        if (!isStillCopying)
        {
            ChangeState(CheatingGameState.IdleSeated);
            return;
        }

        SetCopyingAnimation(true);

        timer += Time.deltaTime;

        if (timer >= incrementInterval)
        {
            timer = 0f;
            IncreaseGrade();
        }
    }

    private void HandleFinishedState()
    {
        SetCopyingAnimation(false);

        if (loadNextSceneWhenDone)
        {
            //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene("Hol2");
        }
    }

    private void HandleCaughtState()
    {
        SetCopyingAnimation(false);
        timer = 0f;
    }

    public void StartSitting()
    {
        if (currentState != CheatingGameState.FreeRoam)
            return;

        ChangeState(CheatingGameState.Sitting);
    }

    private void SitDown()
    {
        ActivateObject(seatedStudent, seatedStudentSortingOrder);
        ActivateObject(deskOn, deskOnSortingOrder);

        if (deskOff != null)
            deskOff.SetActive(false);

        if (player != null)
            player.SetActive(false);

        if (chair != null)
            chair.SetActive(false);
    }

    private void ActivateObject(GameObject obj, int sortingOrder)
    {
        if (obj == null)
            return;

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        obj.SetActive(true);
    }

    private bool ProfessorCanCatch()
    {
        if (professorPatrol == null)
            return false;

        return professorPatrol.IsInBottomInterval;
    }

    private void StartCaughtSequence()
    {
        if (caughtSequenceStarted)
            return;

        caughtSequenceStarted = true;

        ChangeState(CheatingGameState.Caught);

        SetCopyingAnimation(false);

        if (seatedStudentAnimator != null)
        {
            seatedStudentAnimator.SetTrigger(caughtTriggerName);
        }

        if (professorPatrol != null)
        {
            professorPatrol.StartCatchSequence(OnProfessorCatchAnimationFinished);
        }
        else
        {
            OnProfessorCatchAnimationFinished();
        }
    }

    private void OnProfessorCatchAnimationFinished()
    {
        SetCaughtFinalAnimation(true);

        if (tryAgainButton != null)
            tryAgainButton.SetActive(true);
    }

    private void IncreaseGrade()
    {
        grade += 0.01f;

        if (grade > maxGrade)
            grade = maxGrade;

        UpdateGradeText();

        if (grade >= maxGrade)
        {
            ChangeState(CheatingGameState.Finished);
        }
    }

    private void UpdateGradeText()
    {
        if (gradeText != null)
        {
            gradeText.text = grade.ToString("00.00");
        }
    }

    private void SetCopyingAnimation(bool isCopying)
    {
        if (seatedStudentAnimator != null)
        {
            seatedStudentAnimator.SetBool(copyingBoolName, isCopying);
        }

        if (pcAnimator != null)
        {
            pcAnimator.SetBool(copyingBoolName, isCopying);
        }
    }

    private void SetCaughtFinalAnimation(bool isCaughtFinal)
    {
        if (seatedStudentAnimator != null)
        {
            seatedStudentAnimator.SetBool(caughtFinalBoolName, isCaughtFinal);
        }
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ChangeState(CheatingGameState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }
}