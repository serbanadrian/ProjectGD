using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public enum ObjectType { W, A, S, D }

    public ObjectType objectType;
    public float fallSpeed = 3f;

    private float bottomY = -6f;
    private bool hasBeenHandled = false;

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