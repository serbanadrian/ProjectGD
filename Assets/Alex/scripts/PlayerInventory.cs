using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemType> items = new List<ItemType>();

    public void AddItem(ItemType item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }
    }

    public bool HasItem(ItemType item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(ItemType item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public bool HasOnly(ItemType item)
    {
        return items.Count == 1 && items.Contains(item);
    }
}