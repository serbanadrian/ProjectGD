using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public ItemType itemType;

    public void Collect(PlayerInventory inventory)
    {
        inventory.AddItem(itemType);

        Debug.Log("Ai luat: " + itemType);

        gameObject.SetActive(false);
    }
}