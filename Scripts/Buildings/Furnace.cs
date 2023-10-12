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

    public InternalSlot smeltingStorage = new();
    public InternalSlot outputStorage = new();

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        data.storages.Add(new SaveSlot(smeltingStorage));
        data.storages.Add(new SaveSlot(outputStorage));
        data.storages.Add(new SaveSlot(nextFinalResult, 1));
        return data;
    }

    public override void LoadData(SaveBuilding data)
    {
        BaseLoadData(data);
        smeltingStorage = data.storages[0].ToSlot();
        outputStorage = data.storages[1].ToSlot();
        nextFinalResult = SaveManager.i.GetItemFromID(data.storages[2].itemID);
        startTime = Time.time;
        if (nextFinalResult != null)
        {
            isWorking = true;
            spriteRenderer.sprite = onSprite;
            thisLight.enabled = true;
            Invoke(nameof(Smelt), nextFinalResult.craftTime);
        }
    }

    public override void BuildingDestroyed()
    {
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            Inventory.i.Close();
        }
        Inventory.i.DropMultiple(smeltingStorage, transform.position);
        Inventory.i.DropMultiple(outputStorage, transform.position);
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
        InternalSlot slot = new InternalSlot(outputStorage);
        outputStorage.amount = 0;
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            fInt.outputSlot.amount = outputStorage.amount;
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
        return (smeltingStorage.item == null||(smeltingStorage.item.ID == item.ID && smeltingStorage.amount <= smeltingStorage.item.stackSize) 
            || smeltingStorage.amount == 0) && item.smeltedVersion != null;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        smeltingStorage.item = item;
        smeltingStorage.amount += amount;
        if (smeltingStorage.amount <= smeltingStorage.item.stackSize)
        {
            if (fInt.isOpen && fInt.currentFurnace == this)
            {
                fInt.inputSlot.SetItem(item,smeltingStorage.amount);
            }
            return 0;
        } else
        {
            int temp = smeltingStorage.amount - smeltingStorage.item.stackSize;
            smeltingStorage.amount = smeltingStorage.item.stackSize;
            if (fInt.isOpen && fInt.currentFurnace == this)
            {
                fInt.inputSlot.SetItem(item, smeltingStorage.amount);
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
        if (smeltingStorage.amount <= 0 || !hasEnergy)
        {
            spriteRenderer.sprite = offSprite;
            thisLight.enabled = false;
        }
        outputStorage.item = nextFinalResult;
        outputStorage.amount++;
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            fInt.outputSlot.SetItem(outputStorage.item, outputStorage.amount);
            fInt.progressSlider.value = 0;
        }
        isWorking = false;
    }

    private void Update()
    {
        if (fInt.isOpen && fInt.currentFurnace == this)
        {
            if (fInt.inputSlot.item != smeltingStorage.item)
            {
                if (fInt.inputSlot.item != null)
                {
                    smeltingStorage.item = fInt.inputSlot.item;
                    smeltingStorage.amount = fInt.inputSlot.amount;
                    
                }
            } else if (fInt.inputSlot.amount != smeltingStorage.amount)
            {
                smeltingStorage.amount = fInt.inputSlot.amount;
            }
            if (fInt.outputSlot.amount != outputStorage.amount)
            {
                outputStorage.amount = fInt.outputSlot.amount;
            }
            if (isWorking)
            {
                //x:1=(Time.time-startTime):nextFinalResult.craftTime
                float x = (Time.time - startTime) / nextFinalResult.craftTime;
                fInt.progressSlider.value = x;
            }
        }
        if (smeltingStorage.amount > 0 && !isWorking && hasEnergy)
        {
            if (outputStorage.amount < smeltingStorage.item.smeltedVersion.stackSize)
            {
                if (outputStorage.item == null || outputStorage.item == smeltingStorage.item.smeltedVersion 
                    || outputStorage.amount == 0)
                {
                    smeltingStorage.amount--;
                    if (fInt.isOpen && fInt.currentFurnace == this)
                    {
                        fInt.inputSlot.amount = smeltingStorage.amount;
                        fInt.inputSlot.RefreshGraphics();
                    }
                    isWorking = true;
                    spriteRenderer.sprite = onSprite;
                    thisLight.enabled = true;
                    nextFinalResult = smeltingStorage.item.smeltedVersion;
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
