using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Building
{
    public List<InternalSlot> internalSlots=new();
    StorageInterface sInt;

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        foreach (InternalSlot slot in internalSlots)
        {
            if (slot.isEmpty) continue;
            data.storages.Add(new SaveSlot(slot));
        }
        return data;
    }

    public override void LoadData(SaveBuilding data)
    {
        BaseLoadData(data);
        foreach (SaveSlot slot in data.storages)
        {
            if (slot.isEmpty) return;
            PutResource(SaveManager.i.GetItemFromID(slot.itemID), slot.amount);
        }
    }

    public override void BuildingDestroyed()
    {
        if (sInt.isOpen && sInt.currentStorage == this)
        {
            Inventory.i.Close();
        }
        Inventory.i.DropSlots(internalSlots, transform.position);
    }

    public override void FinishInit()
    {
        sInt = constants.storageInterface;
        for (int i = 0; i < 35; i++)
        {
            internalSlots.Add(new InternalSlot());
        }
        AddLight();
        thisLight.pointLightOuterRadius = 5;
        thisLight.intensity = 0.5f;
        thisLight.color =  Color.cyan;
        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        checkpoints.Add(new Checkpoint(this, CheckpointType.Take));
        AddCheckpoints();
    }

    public void Empty()
    {
        foreach (InternalSlot slot in internalSlots)
        {
            slot.Empty();
        }
    }

    public override void OnInteract()
    {
        sInt.OnStorageOpen(this);
    }

    public override bool ShowWorkingStatus()
    {
        return false;
    }

    public override InternalSlot GetResource()
    {
        if (sInt.isOpen && sInt.currentStorage == this)
        {
            return null;
        }
        InternalSlot slot = new InternalSlot();
        foreach (InternalSlot intSlot in internalSlots)
        {
            if (!intSlot.isEmpty)
            {
                slot.item = intSlot.item;
                slot.amount = intSlot.amount;
                intSlot.Empty();
                break;
            }
        }
        if (!slot.isEmpty)
        {
            return slot;
        } else
        {
            return null;
        }
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        if (sInt.isOpen && sInt.currentStorage == this)
        {
            return false;
        }
        foreach (InternalSlot slot in internalSlots)
        {
            if (slot.isEmpty || slot.item.ID == item.ID)
            {
                return true;
            }
        }
        return false;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        int toStore = amount;
        foreach (InternalSlot slot in internalSlots)
        {
            if (!slot.isEmpty && !slot.isFull)
            {
                if (slot.item.ID == item.ID)
                {
                    int remaining = slot.AddAmount(toStore);
                    if (remaining == 0)
                    {
                        RefreshInterface();
                        return 0;
                    }
                    toStore = remaining;
                    
                }
            }
        }
        foreach (InternalSlot slot in internalSlots)
        {
            if (slot.isEmpty)
            {
                slot.item = item;
                int remaining = slot.AddAmount(toStore);
                if (remaining == 0)
                {
                    RefreshInterface(); 
                    return 0;
                }
                toStore = remaining;
                
            }
        }
        RefreshInterface();
        return toStore;
    }

    void RefreshInterface()
    {
        if (sInt.isOpen && sInt.currentStorage == this)
        {
            sInt.Empty();
            sInt.Copy();
        }
    }
}
