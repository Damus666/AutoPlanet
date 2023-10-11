using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDrop
{
    public Vector3 position;
    public int itemID;

    public SaveDrop(int itemID, Vector3 position)
    {
        this.position = position;
        this.itemID = itemID;
    }

    public SaveDrop(Item item, Vector3 position)
    {
        this.position = position;
        itemID = item.ID;
    }
}
