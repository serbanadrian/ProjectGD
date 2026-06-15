using System.Collections;
using UnityEngine;

public class AlarmSequenceManager : MonoBehaviour
{
    [Header("Alarm")]
    public AlarmManager alarmManager;

    [Header("Professor Spawn")]
    public GameObject professorPrefab;
    public Transform professorSpawnPoint;

    [Header("Cage")]
    public CageManager cageManager;

    [Header("Bull")]
    public BullManager bullManager;

    [Header("Barricade")]
    public BarricadeManager barricadeManager;

    [Header("Timing")]
    public float delayAfterAlarmStopped = 1f;

    private bool sequenceStarted = false;
    private bool professorReachedAlarm = false;
    private ProfessorManager spawnedProfessorManager;

    public void StartAlarmSequence()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;

        if (alarmManager != null)
            alarmManager.StartAlarm();

        SpawnProfessor();
    }

    private void SpawnProfessor()
    {
        if (professorPrefab == null || professorSpawnPoint == null)
        {
            Debug.LogWarning("Professor Prefab sau Professor Spawn Point lipseste.");
            return;
        }

        GameObject professor = Instantiate(
            professorPrefab,
            professorSpawnPoint.position,
            professorSpawnPoint.rotation
        );

        spawnedProfessorManager = professor.GetComponent<ProfessorManager>();

        if (spawnedProfessorManager == null)
            Debug.LogWarning("Prefab-ul profesorului nu are ProfessorManager pe el.");
    }

    public void ProfessorReachedAlarm()
    {
        if (professorReachedAlarm)
            return;

        professorReachedAlarm = true;

        StartCoroutine(ProfessorReachedAlarmRoutine());
    }

    private IEnumerator ProfessorReachedAlarmRoutine()
    {
        if (alarmManager != null)
            alarmManager.StopAlarm();

        yield return new WaitForSeconds(delayAfterAlarmStopped);

        if (spawnedProfessorManager != null)
            spawnedProfessorManager.PlayNoticeBull();

        if (cageManager != null)
            cageManager.OpenCage();

        if (bullManager != null)
        {
            bullManager.barricadeManager = barricadeManager;
            bullManager.StartBull();
        }
    }
}