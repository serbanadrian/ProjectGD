using UnityEngine;

public class ArduinoBoardAlarmTrigger : MonoBehaviour
{
    public AlarmSequenceManager alarmSequenceManager;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        ProfessorManager professor = other.GetComponent<ProfessorManager>();

        if (professor != null)
        {
            triggered = true;

            if (alarmSequenceManager != null)
                alarmSequenceManager.ProfessorReachedAlarm();
        }
    }
}