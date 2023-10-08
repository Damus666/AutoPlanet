using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Color canCraftColor = Color.green;
    [SerializeField] Color cannorCraftColor = Color.red;
    [SerializeField] Color canCraftColorHovered = Color.green;
    [SerializeField] Color cannorCraftColorHovered = Color.red;
    [SerializeField] Image outline;

    InfoBoxManager infoBoxManager;
    Inventory inventory;
    CraftingQueue queue;
    UnlockManager unlockManager;

    bool isHovering;
    bool canCraft;
    string craftError = "Can craft";
    bool isUnlocked = true;
    public Item item;

    public void Setup(Item item, Inventory inv, CraftingQueue queue)
    {
        infoBoxManager = GameObject.Find("GameManager").GetComponent<InfoBoxManager>();
        this.item = item;
        itemImage.sprite = item.texture;
        inventory = inv;
        this.queue = queue;
        unlockManager = inventory.GetComponent<UnlockManager>();
    }

    private void Update()
    {
        if (isHovering)
        {
            infoBoxManager.SetFromItem(item);
            infoBoxManager.SetFromCraftingInfo(item, craftError);
        }
        CheckCanCraft();
    }

    public void POINTERENTER()
    {
        isHovering = true;
        UpdateOutline();
    }

    public void POINTEREXIT()
    {
        isHovering = false;
        UpdateOutline();
    }

    public void POINTERCLICK()
    {
        StartCrafting();
    }

    public void StartCrafting()
    {
        if (canCraft)
        {
            queue.AddToQueue(item);
            foreach (Requirement req in item.requirements)
            {
                inventory.RemoveItem(req.item, req.amount);
            }
            CheckCanCraft();
        }
    }

    public void CheckCanCraft()
    {
        if (!unlockManager.unlockedIDs.Contains(item.ID))
        {
            canCraft = false;
            craftError = "Locked";
            isUnlocked = false;
            UpdateOutline();
            return;
        } else
        {
            isUnlocked = true;
        }
        if (item.carftType == CraftType.OnlyHands || item.carftType == CraftType.HandsAndCrafter)
        {
            if (inventory.CheckRequirements(item.requirements))
            {
                canCraft = true;
                craftError = "Can craft";
            } else
            {
                canCraft = false;
                craftError = "Missing resources";
            }
        } else
        {
            canCraft = false;
            if (item.carftType == CraftType.OnlyCrafter)
            {
                craftError = "Can only be crafted with the crafter";
            } else if (item.carftType == CraftType.OnlySmelting)
            {
                craftError = "Can only be obtained with the furnace";
            }
        }
        UpdateOutline();
    }

    public void UpdateOutline()
    {
        if (isHovering)
        {
            if (canCraft)
            {
                outline.color = canCraftColorHovered;
            } else
            {
                outline.color = cannorCraftColorHovered;
            }
        } else
        {
            if (canCraft)
            {
                outline.color = canCraftColor;
            }
            else
            {
                outline.color = cannorCraftColor;
            }
        }
        if (isUnlocked)
        {
            if (itemImage.color.a < 1)
            {
                itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1);
            }
        } else
        {
            if (itemImage.color.a >= 1)
            {
                itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0.3f);
            }
        }
    }
}
