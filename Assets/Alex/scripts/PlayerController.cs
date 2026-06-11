using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 4f;

    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimation();
    }

    private void MovePlayer()
    {
        if (IsLockedInSpecialAnimation())
            return;

        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.position += movement * speed * Time.deltaTime;

        if (moveInput != Vector2.zero)
            lastDirection = moveInput;
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        if (IsLockedInSpecialAnimation())
            return;

        if (moveInput != Vector2.zero)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                    animator.Play("walk_east");
                else
                    animator.Play("walk_west");
            }
            else
            {
                if (moveInput.y > 0)
                    animator.Play("walk_north");
                else
                    animator.Play("walk_south");
            }
        }
        else
        {
            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                if (lastDirection.x > 0)
                    animator.Play("idle_east");
                else
                    animator.Play("idle_west");
            }
            else
            {
                if (lastDirection.y > 0)
                    animator.Play("idle_north");
                else
                    animator.Play("idle_south");
            }
        }
    }

    private bool IsLockedInSpecialAnimation()
    {
        if (animator == null)
            return false;

        if (animator.GetBool("ThreateningDog"))
            return true;

        if (animator.GetBool("isSitting"))
            return true;

        return false;
    }

    public void OnMove_arcade(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}