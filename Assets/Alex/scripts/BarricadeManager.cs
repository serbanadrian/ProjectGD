using UnityEngine;

public class BarricadeManager : MonoBehaviour
{
    private bool destroyed = false;

    public void DestroyBarricade()
    {
        if (destroyed)
            return;

        destroyed = true;
        gameObject.SetActive(false);
    }
}