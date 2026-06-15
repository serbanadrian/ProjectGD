using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BullManager : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public Vector2 moveDirection = Vector2.right;

    [Header("Target")]
    public BarricadeManager barricadeManager;

    private bool started = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void StartBull()
    {
        if (started)
            return;

        started = true;
        StartCoroutine(BullMoveRoutine());
    }

    private IEnumerator BullMoveRoutine()
    {
        while (started)
        {
            Vector2 newPosition =
                rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProfessorManager professor = other.GetComponent<ProfessorManager>();

        if (professor != null)
        {
            professor.ThrowAway();
            return;
        }

        BarricadeManager barricade = other.GetComponent<BarricadeManager>();

        if (barricade != null)
        {
            barricade.DestroyBarricade();

            // NU oprim taurul aici.
            // Taurul continua sa mearga dupa ce sparge baricada.
            return;
        }
    }
}