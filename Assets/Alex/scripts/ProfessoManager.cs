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
    public string walkRightAnim = "walk_west";
    public string idleNorthAnim = "idle_north";
    public string noticeBullAnim = "bull_incoming";

    [Header("Timing")]
    public float noticeAlarmDuration = 1f;

    private GameObject professorPrefab;
    private Transform professorSpawnPoint;

    private GameObject currentProfessor;
    private Animator professorAnimator;

    public void InitializeProfessor(GameObject prefab, Transform spawnPoint)
    {
        professorPrefab = prefab;
        professorSpawnPoint = spawnPoint;
    }

    public IEnumerator StartProfessorAlarmSequence()
    {
        SpawnProfessor();

        if (currentProfessor == null)
            yield break;

        PlayAnimation(noticeAlarmAnim);

        yield return new WaitForSeconds(noticeAlarmDuration);

        PlayAnimation(walkNorthAnim);
        yield return MoveForTime(Vector2.up, walkUpTime);

        PlayAnimation(walkRightAnim);
        yield return MoveForTime(Vector2.right, walkRightTime);

        PlayAnimation(idleNorthAnim);
    }

    public void InitializeProfessor(GameObject professor)
    {
        currentProfessor = professor;

        if (currentProfessor != null)
        {
            professorAnimator = currentProfessor.GetComponent<Animator>();
        }
    }

    private void SpawnProfessor()
    {
        if (currentProfessor != null)
            return;

        if (professorPrefab == null)
        {
            Debug.LogWarning("Professor Prefab lipseste in AlarmSequenceManager.");
            return;
        }

        if (professorSpawnPoint == null)
        {
            Debug.LogWarning("Professor Spawn Point lipseste in AlarmSequenceManager.");
            return;
        }

        currentProfessor = Instantiate(
            professorPrefab,
            professorSpawnPoint.position,
            professorSpawnPoint.rotation
        );

        professorAnimator = currentProfessor.GetComponent<Animator>();

        if (professorAnimator == null)
        {
            Debug.LogWarning("Profesorul spawnat nu are Animator.");
        }
    }

    private IEnumerator MoveForTime(Vector2 direction, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (currentProfessor == null)
                yield break;

            currentProfessor.transform.position +=
                (Vector3)(direction.normalized * moveSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayNoticeBull()
    {
        PlayAnimation(noticeBullAnim);
    }

    public void HideProfessor()
    {
        if (currentProfessor != null)
        {
            currentProfessor.SetActive(false);
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (professorAnimator != null && !string.IsNullOrEmpty(animationName))
        {
            professorAnimator.Play(animationName);
        }
    }
}