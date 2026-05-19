using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public ObjectType objectType;
    public float fallSpeed = 3f;

    private float bottomY = -5f; // Y la care obiectul "a ajuns jos"
    private bool hasBeenHandled = false;

    public enum ObjectType { W, A, S, D }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (!hasBeenHandled && transform.position.y <= bottomY)
        {
            hasBeenHandled = true;
            MashMinigame.Instance?.OnObjectMissed(this);
        }
    }

    public void HandleCorrect()
    {
        if (hasBeenHandled) return;
        hasBeenHandled = true;
        Destroy(gameObject);
    }

    public void HandleMissed()
    {
        Destroy(gameObject);
    }
}