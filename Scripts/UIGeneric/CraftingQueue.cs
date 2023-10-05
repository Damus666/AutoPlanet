using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQueue : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject queueSlotPrefab;
    [SerializeField] Transform slotsParent;

    List<InternalSlot> queue = new();
    List<CraftingQueueSlot> slots = new();
    bool crafting = false;
    Item craftResult;
    float craftStartTime;

    CraftingQueueSlot GetSlot(Item item)
    {
        foreach (CraftingQueueSlot slot in slots)
        {
            if (slot.item.ID == item.ID)
            {
                return slot;
            }
        }
        return null;
    }

    public void AddToQueue(Item item)
    {
        bool found = false;
        foreach (InternalSlot slot in queue)
        {
            if (slot.item.ID == item.ID)
            {
                slot.amount += item.craftAmount;
                found = true;
                CraftingQueueSlot uiSlot = GetSlot(slot.item);
                if (uiSlot != null)
                {
                    uiSlot.RefreshAmount(slot.amount);
                }
                break;
            }
        }
        if (!found)
        {
            queue.Add(new InternalSlot(item, item.craftAmount));
            GameObject slotObj = Instantiate(queueSlotPrefab, slotsParent);
            CraftingQueueSlot uiSlot = slotObj.GetComponent<CraftingQueueSlot>();
            uiSlot.Setup(item, item.craftAmount, this);
            slots.Add(uiSlot);
        }
    }

    public void RemoveFromQueue(Item item)
    {
        for (int i = 0; i < queue.Count; i++)
        {
            InternalSlot slot = queue[i];
            if (slot.item.ID == item.ID)
            {
                slot.amount -= item.craftAmount;
                if (slot.amount <= 0)
                {
                    queue.Remove(slot);
                    CraftingQueueSlot uiSlot = GetSlot(item);
                    if (uiSlot != null)
                    {
                        Destroy(uiSlot.gameObject);
                        slots.Remove(uiSlot);
                    }
                } else
                {
                    CraftingQueueSlot uiSlot = GetSlot(slot.item);
                    if (uiSlot != null)
                    {
                        uiSlot.RefreshAmount(slot.amount);
                    }
                }
                break;
            }
        }
    }

    public void OnSlotClick(Item item)
    {
        RemoveFromQueue(item);
        if (item == craftResult)
        {
            crafting = false;
        }
        foreach (Requirement req in item.requirements)
        {
            inventory.AddItem(req.item, req.amount);
        }
    }

    public void StartCrafting()
    {
        ResetUIProgress();
        craftResult = queue[0].item;
        crafting = true;
        craftStartTime = Time.time;
        Invoke(nameof(FinishCrafting), craftResult.craftTime);
    }

    void ResetUIProgress()
    {
        foreach (CraftingQueueSlot slot in slots)
        {
            slot.ResetProgress();
        }
    }

    public void FinishCrafting()
    {
        ResetUIProgress();
        if (!crafting) return;
        crafting = false;
        inventory.AddItem(craftResult, craftResult.craftAmount);
        RemoveFromQueue(craftResult);
    }

    private void Update()
    {
        if (!crafting && queue.Count > 0)
        {
            StartCrafting();
        } else if (crafting)
        {
            float fillAmount = (Time.time-craftStartTime)/craftResult.craftTime;
            CraftingQueueSlot uiSlot = GetSlot(craftResult);
            if (uiSlot != null)
            {
                uiSlot.UpdateProgress(fillAmount);
            }
        }
    }
}
