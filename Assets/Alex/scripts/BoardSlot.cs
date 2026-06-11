using UnityEngine;

[System.Serializable]
public class BoardSlot
{
    public ItemType itemType;
    public Transform spawnPoint;
    public GameObject prefab;

    [HideInInspector]
    public bool isPlaced;
}