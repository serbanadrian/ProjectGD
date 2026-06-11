using UnityEngine;

public class PlaceComponentCommand : IBoardCommand
{
    private BoardSlot slot;
    private PlayerInventory inventory;

    public PlaceComponentCommand(BoardSlot slot, PlayerInventory inventory)
    {
        this.slot = slot;
        this.inventory = inventory;
    }

    public void Execute()
    {
        if (slot == null || inventory == null)
            return;

        if (slot.prefab != null && slot.spawnPoint != null)
        {
            GameObject obj = Object.Instantiate(
                slot.prefab,
                slot.spawnPoint.position,
                slot.spawnPoint.rotation
            );

            obj.transform.localScale = slot.prefab.transform.localScale;

            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in renderers)
            {
                sr.sortingLayerName = "table";
                sr.sortingOrder = 20;
            }
        }
        else
        {
            Debug.LogWarning("Slotul pentru " + slot.itemType + " nu are prefab sau spawn point.");
        }

        slot.isPlaced = true;
        inventory.RemoveItem(slot.itemType);

        Debug.Log("Componenta montata pe board: " + slot.itemType);
    }
}