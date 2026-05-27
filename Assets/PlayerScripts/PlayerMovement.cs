using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;

    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Citim input-ul de la tastatură
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D sau săgeți stânga/dreapta
        movement.y = Input.GetAxisRaw("Vertical");   // W/S sau săgeți sus/jos

        // Normalizăm ca să nu meargă mai repede pe diagonală
        movement = movement.normalized;

        // Dacă există mișcare, pornim animația
        bool isMoving = movement.magnitude > 0;

        animator.SetBool("isMoving", isMoving);
    }

    void FixedUpdate()
    {
        // Mutăm playerul
        transform.position += (Vector3)(movement * speed * Time.fixedDeltaTime);
    }
}