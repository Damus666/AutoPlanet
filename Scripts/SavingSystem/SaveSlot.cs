using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSlot
{
    public int itemID;
    public int amount;

    public bool isEmpty { get { return amount <= 0 || itemID == -1; } }

    public InternalSlot ToSlot(SaveManager manager)
    {
        return new InternalSlot(manager.GetItemFromID(itemID), amount);
    }

    public SaveSlot(Item item, int amount)
    {
        if (item != null)
            itemID = item.ID;
        else
            itemID = -1;
        this.amount = amount;
    }

    public SaveSlot(Slot slot)
    {
        if (!slot.isEmpty)
            itemID = slot.item.ID;
        else
            itemID = -1;
        amount = slot.amount;
    }

    public SaveSlot(InternalSlot slot)
    {
        if (!slot.isEmpty)
            itemID = slot.item.ID;
        else
            itemID = -1;
        amount = slot.amount;
    }
}
