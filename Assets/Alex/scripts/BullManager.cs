using System.Collections;
using UnityEngine;

public class BullManager : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public Vector2 moveDirection = Vector2.right;
    public float moveDuration = 3f;

    [Header("Target")]
    public BarricadeManager barricadeManager;

    private bool started = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartBull()
    {
        if (started)
            return;

        started = true;
        gameObject.SetActive(true);
        StartCoroutine(BullMoveRoutine());
    }

    private IEnumerator BullMoveRoutine()
    {
        float timer = 0f;

        while (timer < moveDuration)
        {
            transform.position +=
                (Vector3)(moveDirection.normalized * moveSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        if (barricadeManager != null)
        {
            barricadeManager.DestroyBarricade();
        }
    }
}