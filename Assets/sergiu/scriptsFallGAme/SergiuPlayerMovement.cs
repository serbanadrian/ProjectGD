using UnityEngine;
using UnityEngine.InputSystem;

public class SergiuPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 inputDirection;
    private Vector2 currentVelocity;

    // Animator parameter hashes
    private static readonly int AnimMoveX = Animator.StringToHash("MoveX");
    private static readonly int AnimMoveY = Animator.StringToHash("MoveY");
    private static readonly int AnimSpeed  = Animator.StringToHash("Speed");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // optional
    }

    void Update()
    {
        // Noul Input System
        Vector2 rawInput = Keyboard.current != null ? new Vector2(
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) -
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed  ? 1f : 0f),
            (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed    ? 1f : 0f) -
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed  ? 1f : 0f)
        ) : Vector2.zero;

        inputDirection = rawInput.normalized;

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;

        float lerpSpeed = (inputDirection.sqrMagnitude > 0f) ? acceleration : deceleration;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, lerpSpeed * Time.fixedDeltaTime);

        rb.linearVelocity = currentVelocity;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat(AnimMoveX, inputDirection.x);
        animator.SetFloat(AnimMoveY, inputDirection.y);
        animator.SetFloat(AnimSpeed, inputDirection.sqrMagnitude);
    }
}