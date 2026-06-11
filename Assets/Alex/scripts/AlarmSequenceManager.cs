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
    public float delayBeforeCageAndBull = 2f;

    private bool sequenceStarted = false;

    public void StartAlarmSequence()
    {
        if (sequenceStarted)
            return;

        sequenceStarted = true;
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        if (alarmManager != null)
            alarmManager.StartAlarm();

        if (professorPrefab != null && professorSpawnPoint != null)
        {
            Instantiate(
                professorPrefab,
                professorSpawnPoint.position,
                professorSpawnPoint.rotation
            );
        }

        yield return new WaitForSeconds(delayBeforeCageAndBull);

        if (cageManager != null)
            cageManager.OpenCage();

        if (bullManager != null)
        {
            bullManager.barricadeManager = barricadeManager;
            bullManager.StartBull();
        }
    }
}