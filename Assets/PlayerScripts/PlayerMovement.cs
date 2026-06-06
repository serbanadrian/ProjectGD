using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;

    private Animator animator;

    private Vector2 movement;
    private Vector2 animationDirection;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f; // default: față / jos

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("moveX", 0f);
        animator.SetFloat("moveY", -1f);
        animator.SetFloat("lastMoveX", lastMoveX);
        animator.SetFloat("lastMoveY", lastMoveY);
    }

    void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // Mișcarea reală permite diagonală
        movement = new Vector2(inputX, inputY).normalized;

        // Direcția pentru animație
        animationDirection = GetAnimationDirection(inputX, inputY);

        bool isMoving = movement.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            lastMoveX = animationDirection.x;
            lastMoveY = animationDirection.y;
        }
    }

    private Vector2 GetAnimationDirection(float inputX, float inputY)
    {
        // Dacă merge diagonal, prioritizăm stânga/dreapta pentru animație
        if (inputX != 0)
        {
            return new Vector2(inputX, 0f).normalized;
        }

        // Dacă nu merge diagonal, folosim sus/jos
        if (inputY != 0)
        {
            return new Vector2(0f, inputY).normalized;
        }

        // Dacă nu se mișcă, păstrăm ultima direcție
        return new Vector2(lastMoveX, lastMoveY);
    }

    private void UpdateAnimator()
    {
        bool isMoving = movement.sqrMagnitude > 0.01f;

        animator.SetBool("isMoving", isMoving);

        animator.SetFloat("moveX", animationDirection.x);
        animator.SetFloat("moveY", animationDirection.y);

        animator.SetFloat("lastMoveX", lastMoveX);
        animator.SetFloat("lastMoveY", lastMoveY);
    }

    private void MovePlayer()
    {
        transform.position += (Vector3)(movement * speed * Time.fixedDeltaTime);
    }
}