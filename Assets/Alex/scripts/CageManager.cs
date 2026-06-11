using System.Collections;
using UnityEngine;

public class CageManager : MonoBehaviour
{
    [Header("Lift Settings")]
    public float liftSpeed = 2f;
    public float liftDuration = 2f;

    private bool isOpening = false;

    public void OpenCage()
    {
        if (isOpening)
            return;

        isOpening = true;
        StartCoroutine(LiftCageRoutine());
    }

    private IEnumerator LiftCageRoutine()
    {
        float timer = 0f;

        while (timer < liftDuration)
        {
            transform.position += Vector3.up * liftSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}