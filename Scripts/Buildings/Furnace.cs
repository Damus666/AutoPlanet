using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Building
{
    Sprite onSprite;
    Sprite offSprite;
    FurnaceInterface fInt;
    Item nextFinalResult;
    float startTime;

    public Item itemSmelting;
    public int amount;
    public Item outputItem;
    public int outputAmount;

    public override void BuildingDestroyed(Inventory inventory)
    {
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            inventory.Close();
        }
        inventory.DropMultiple(itemSmelting, amount, transform.position);
        inventory.DropMultiple(outputItem, outputAmount, transform.position);
    }

    public override void FinishInit()
    {
        offSprite = spriteRenderer.sprite;
        onSprite = constants.furnaceOnSprite;
        fInt = constants.furnaceInterface;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.medium;
        thisLight.color = new Color (230f/255f, 140f/255f, 0,1);
        thisLight.enabled = false;
        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        checkpoints.Add(new Checkpoint(this, CheckpointType.Take));
        AddCheckpoints();
    }

    public override InternalSlot GetResource()
    {
        InternalSlot slot = new InternalSlot(outputItem, outputAmount);
        outputAmount = 0;
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            fInt.outputSlot.amount = outputAmount;
            fInt.outputSlot.RefreshGraphics();
        }
        if (slot.isEmpty)
        {
            return null;
        }
        return slot;
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        return (itemSmelting == null||(itemSmelting.ID == item.ID && amount <= itemSmelting.stackSize) || amount == 0) && item.smeltedVersion != null;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        itemSmelting = item;
        this.amount += amount;
        if (this.amount <= itemSmelting.stackSize)
        {
            if (fInt.isOpen && fInt.currentFurnace == this)
            {
                fInt.inputSlot.SetItem(item,this.amount);
            }
            return 0;
        } else
        {
            int temp = this.amount - itemSmelting.stackSize;
            this.amount = itemSmelting.stackSize;
            if (fInt.isOpen && fInt.currentFurnace == this)
            {
                fInt.inputSlot.SetItem(item, this.amount);
            }
            return temp;
        }
    }

    public override void OnInteract()
    {
        fInt.OnOpen(this);
    }

    void Smelt()
    {
        if (amount <= 0 || !hasEnergy)
        {
            spriteRenderer.sprite = offSprite;
            thisLight.enabled = false;
        }
        outputItem = nextFinalResult;
        outputAmount++;
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            fInt.outputSlot.SetItem(outputItem, outputAmount);
            fInt.progressSlider.value = 0;
        }
        isWorking = false;
    }

    private void Update()
    {
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            if (fInt.inputSlot.item != itemSmelting)
            {
                if (fInt.inputSlot.item != null)
                {
                    itemSmelting = fInt.inputSlot.item;
                    amount = fInt.inputSlot.amount;
                    
                }
            } else if (fInt.inputSlot.amount != amount)
            {
                amount = fInt.inputSlot.amount;
            }
            if (fInt.outputSlot.amount != outputAmount)
            {
                outputAmount = fInt.outputSlot.amount;
            }
            if (isWorking)
            {
                //x:1=(Time.time-startTime):nextFinalResult.craftTime
                float x = (Time.time - startTime) / nextFinalResult.craftTime;
                fInt.progressSlider.value = x;
            }
        }
        if (amount > 0 && !isWorking && hasEnergy)
        {
            if (outputAmount < itemSmelting.smeltedVersion.stackSize)
            {
                if (outputItem == null || outputItem == itemSmelting.smeltedVersion || outputAmount == 0)
                {
                    amount--;
                    if (fInt.isOpen && fInt.currentFurnace == this)
                    {
                        fInt.inputSlot.amount = amount;
                        fInt.inputSlot.RefreshGraphics();
                    }
                    isWorking = true;
                    spriteRenderer.sprite = onSprite;
                    thisLight.enabled = true;
                    nextFinalResult = itemSmelting.smeltedVersion;
                    startTime = Time.time;
                    Invoke(nameof(Smelt), nextFinalResult.craftTime);
                }
            }
        } else
        {
            if (isWorking && !hasEnergy)
            {
                isWorking = false;
                spriteRenderer.sprite = offSprite;
                thisLight.enabled = false;
            }
        }
    }
}
