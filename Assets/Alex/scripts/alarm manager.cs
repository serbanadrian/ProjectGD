using UnityEngine;

public class AlarmManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource alarmAudioSource;

    [Header("Optional Animation")]
    public Animator alarmAnimator;
    public string alarmOnAnimation = "alarm_on";

    public void StartAlarm()
    {
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Play();
        }

        if (alarmAnimator != null && !string.IsNullOrEmpty(alarmOnAnimation))
        {
            alarmAnimator.Play(alarmOnAnimation);
        }
    }

    public void StopAlarm()
    {
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();
        }
    }
}