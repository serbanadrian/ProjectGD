using System.Collections;
using UnityEngine;

public class ProfessorManager : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float walkUpTime = 1.5f;
    public float walkRightTime = 2f;

    [Header("Animations")]
    public string noticeAlarmAnim = "notice_alarm";
    public string walkNorthAnim = "walk_north";
    public string walkRightAnim = "walk_east";
    public string idleNorthAnim = "idle_north";
    public string noticeBullAnim = "notice_bull";

    [Header("Timing")]
    public float noticeAlarmDuration = 1f;

    private Animator animator;
    private bool alarmRoutineStarted = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartAlarmRoutine();
    }

    public void StartAlarmRoutine()
    {
        if (alarmRoutineStarted)
            return;

        alarmRoutineStarted = true;
        StartCoroutine(ProfessorAlarmRoutine());
    }

    private IEnumerator ProfessorAlarmRoutine()
    {
        PlayAnimation(noticeAlarmAnim);

        yield return new WaitForSeconds(noticeAlarmDuration);

        PlayAnimation(walkNorthAnim);
        yield return MoveForTime(Vector2.up, walkUpTime);

        PlayAnimation(walkRightAnim);
        yield return MoveForTime(Vector2.right, walkRightTime);

        PlayAnimation(idleNorthAnim);
    }

    private IEnumerator MoveForTime(Vector2 direction, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.position +=
                (Vector3)(direction.normalized * moveSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
    }


    public void ThrowAway()
    {
        StartCoroutine(ThrowAwayRoutine());
    }

    private IEnumerator ThrowAwayRoutine()
    {
        float duration = 1.2f;
        float timer = 0f;

        Vector3 direction = new Vector3(1f, 1f, 0f).normalized;
        float throwSpeed = 5f;
        float spinSpeed = 720f;

        while (timer < duration)
        {
            transform.position += direction * throwSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void PlayNoticeBull()
    {
        PlayAnimation(noticeBullAnim);
    }

    public void HideProfessor()
    {
        gameObject.SetActive(false);
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            Debug.LogWarning("Profesorul nu are Animator.");
            return;
        }

        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("Numele animatiei profesorului este gol.");
            return;
        }

        Debug.Log("Profesor play: " + animationName);
        animator.Play(animationName, 0, 0f);
    }
}