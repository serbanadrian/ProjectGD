using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CheatingGradeCounter : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI gradeText;

    [Header("Grade Settings")]
    public float grade = 0f;
    public float maxGrade = 10f;

    [Tooltip("La câte secunde crește nota cu 0.01")]
    public float incrementInterval = 0.01f;

    [Header("Scene")]
    public bool loadNextSceneWhenDone = true;

    private float timer = 0f;
    private bool canCheat = false;
    private bool finished = false;

    private void Start()
    {
        UpdateGradeText();
    }

    private void Update()
    {
        if (!canCheat || finished)
            return;

        bool isCheating = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

        if (isCheating)
        {
            timer += Time.deltaTime;

            if (timer >= incrementInterval)
            {
                timer = 0f;
                IncreaseGrade();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void StartCheating()
    {
        canCheat = true;
    }

    private void IncreaseGrade()
    {
        grade += 0.01f;

        if (grade > maxGrade)
            grade = maxGrade;

        UpdateGradeText();

        if (grade >= maxGrade)
        {
            FinishCheating();
        }
    }

    private void UpdateGradeText()
    {
        if (gradeText != null)
        {
            gradeText.text = grade.ToString("00.00");
        }
    }

    private void FinishCheating()
    {
        finished = true;
        canCheat = false;

        Debug.Log("Nota finală: 10.00");

        if (loadNextSceneWhenDone)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}