using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CopyExamManager : MonoBehaviour
{
    [Header("Objects Before Sitting")]
    public GameObject player;
    public GameObject chair;
    public GameObject deskOff;

    [Header("Object After Sitting")]
    public GameObject sittingStudentSprite;

    [Header("Sorting Order")]
    public int sittingStudentSortingOrder = 2;

    [Header("Copying Animations")]
    public Animator sittingStudentAnimator;
    public Animator boardAnimator;

    [Header("Animation Parameters")]
    public string copyingBoolName = "isCopying";

    [Header("Grade UI")]
    public TextMeshProUGUI gradeText;

    [Header("Grade Settings")]
    public float grade = 0f;
    public float maxGrade = 10f;

    [Tooltip("La câte secunde crește nota cu 0.01")]
    public float incrementInterval = 0.05f;

    [Header("Scene")]
    public string sceneToLoadWhenDone = "Hol4";

    [Header("Professor")]
    public CopyExamProfessorPatrol professorPatrol;

    [Header("Caught Animations")]
    public string caughtTriggerName = "getCaught";
    public string caughtFinalBoolName = "isCaughtFinal";

    [Header("Try Again UI")]
    public GameObject tryAgainButton;

    private bool hasSatDown = false;
    private bool isCopying = false;
    private float timer = 0f;
    private bool sceneLoadStarted = false;
    private bool isCaught = false;

    private void Start()
    {
        if (sittingStudentSprite != null)
        {
            sittingStudentSprite.SetActive(false);
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.SetActive(false);
        }

        SetCopyingAnimation(false);
        UpdateGradeText();

        Debug.Log("CopyExamManager started.");
    }

    private void Update()
    {
        if (!hasSatDown)
            return;

        HandleCopyingInput();
    }

    public void SitDown()
    {
        if (hasSatDown)
            return;

        hasSatDown = true;

        Debug.Log("Player sat down.");

        if (sittingStudentSprite != null)
        {
            SpriteRenderer spriteRenderer = sittingStudentSprite.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = sittingStudentSortingOrder;
            }

            sittingStudentSprite.SetActive(true);
        }

        if (deskOff != null)
        {
            deskOff.SetActive(false);
        }

        if (player != null)
        {
            player.SetActive(false);
        }

        if (chair != null)
        {
            chair.SetActive(false);
        }
    }

private void HandleCopyingInput()
{
    if (isCaught)
        return;

    bool wantsToCopy = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

    if (wantsToCopy && ProfessorCanCatch())
    {
        StartCaughtSequence();
        return;
    }

    isCopying = wantsToCopy;
    SetCopyingAnimation(isCopying);

    if (isCopying)
    {
        IncreaseGradeOverTime();
    }
    else
    {
        timer = 0f;
    }
}

    private void IncreaseGradeOverTime()
    {
        timer += Time.deltaTime;

        if (timer >= incrementInterval)
        {
            timer = 0f;
            IncreaseGrade();
        }
    }

private void IncreaseGrade()
{
    grade += 0.01f;

    if (grade > maxGrade)
    {
        grade = maxGrade;
    }

    UpdateGradeText();

    if (grade >= maxGrade && !sceneLoadStarted)
    {
        sceneLoadStarted = true;
        Debug.Log("Nota a ajuns la 10. Trecem în Hol4.");
        SceneManager.LoadScene(sceneToLoadWhenDone);
    }
}

private bool ProfessorCanCatch()
{
    return professorPatrol != null && professorPatrol.IsOnTopEdge;
}

private void StartCaughtSequence()
{
    if (isCaught)
        return;

    isCaught = true;

    Debug.Log("Student caught cheating.");

    SetCopyingAnimation(false);

    if (sittingStudentAnimator != null)
    {
        sittingStudentAnimator.SetTrigger(caughtTriggerName);
    }

    if (professorPatrol != null)
    {
        professorPatrol.StartCatchSequence(OnProfessorCatchFinished);
    }
    else
    {
        OnProfessorCatchFinished();
    }
}

private void OnProfessorCatchFinished()
{
    if (sittingStudentAnimator != null)
    {
        sittingStudentAnimator.SetBool(caughtFinalBoolName, true);
    }

    if (tryAgainButton != null)
    {
        tryAgainButton.SetActive(true);
    }
}

public void TryAgain()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
    );
}

    private void UpdateGradeText()
    {
        if (gradeText != null)
        {
            gradeText.text = grade.ToString("00.00");
        }
    }

    private void SetCopyingAnimation(bool value)
    {
        if (sittingStudentAnimator != null)
        {
            sittingStudentAnimator.SetBool(copyingBoolName, value);
        }

        if (boardAnimator != null)
        {
            boardAnimator.SetBool(copyingBoolName, value);
        }
    }
}