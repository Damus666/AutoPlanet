using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : Building
{
    Sprite onSprite;
    Sprite offSprite;
    SpriteRenderer itemShower;
    SpriteRenderer itemShowerShadow;

    LaboratoryInterface lInt;
    UnlockManager unlockManager;
    float startTime;

    public InternalSlot storage = new();

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        data.storages.Add(new SaveSlot(storage));
        return data;
    }

    public override void LoadData(SaveBuilding data, SaveManager manager)
    {
        BaseLoadData(data);
        storage = data.storages[0].ToSlot(manager);
        startTime = Time.time;
        if (storage.item != null)
            Invoke(nameof(Process), storage.item.processTime);
    }

    public override void BuildingDestroyed(Inventory inventory)
    {
        if (lInt.isOpen && lInt.currentLab == this)
        {
            inventory.Close();
        }
        inventory.DropMultiple(storage, transform.position);
    }

    public override void FinishInit()
    {
        unlockManager = GameObject.Find("Player").GetComponent<UnlockManager>();
        offSprite = spriteRenderer.sprite;
        onSprite = constants.labOnSprite;
        lInt = constants.laboratoryInterface;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.medium;
        thisLight.color = new Color(1, 0, 1, 1);
        thisLight.enabled = false;
        GameObject itemShowerObj2 = Instantiate(constants.itemShowerPrefab, transform);
        itemShowerObj2.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        itemShowerObj2.transform.localPosition = new Vector3(0, 0.05f, 0);
        GameObject itemShowerObj = Instantiate(constants.itemShowerPrefab, transform);
        itemShowerObj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        itemShowerObj.transform.localPosition = new Vector3(0, 0.05f, 0);
        itemShower = itemShowerObj.GetComponent<SpriteRenderer>();
        itemShowerShadow = itemShowerObj2.GetComponent<SpriteRenderer>();
        itemShowerShadow.color = Color.black;
        itemShowerShadow.sortingOrder -= 1;
        itemShower.gameObject.SetActive(false);
        itemShowerShadow.gameObject.SetActive(false);
        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        AddCheckpoints();
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        return storage.item == null || (storage.item.ID == item.ID && storage.amount <= storage.item.stackSize) || storage.amount == 0;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        storage.item = item;
        storage.amount += amount;
        if (storage.amount <= storage.item.stackSize)
        {
            if (lInt.isOpen && lInt.currentLab == this)
            {
                lInt.inputSlot.SetItem(item, storage.amount);
            }
            return 0;
        }
        else
        {
            int temp = storage.amount - storage.item.stackSize;
            storage.amount = storage.item.stackSize;
            if (lInt.isOpen && lInt.currentLab == this)
            {
                lInt.inputSlot.SetItem(item, storage.amount);
            }
            return temp;
        }
    }

    public override void OnInteract()
    {
        lInt.OnOpen(this);
    }

    void Process()
    {
        if (storage.amount <= 0 || !hasEnergy)
        {
            spriteRenderer.sprite = offSprite;
            thisLight.enabled = false;
            itemShower.gameObject.SetActive(false);
            itemShowerShadow.gameObject.SetActive(false);
        }
        unlockManager.Sell(storage.item.experience);
        if (lInt.isOpen && lInt.currentLab == this)
        {
            lInt.progressSlider.value = 0;
        }
        isWorking = false;
    }

    private void Update()
    {
        if (lInt.isOpen && lInt.currentLab == this)
        {
            if (lInt.inputSlot.item != storage.item)
            {
                if (lInt.inputSlot.item != null)
                {
                    storage.item = lInt.inputSlot.item;
                    storage.amount = lInt.inputSlot.amount;

                }
            }
            else if (lInt.inputSlot.amount != storage.amount)
            {
                storage.amount = lInt.inputSlot.amount;
            }
            if (isWorking)
            {
                //x:1=(Time.time-startTime):nextFinalResult.craftTime
                float x = (Time.time - startTime) / storage.item.processTime;
                lInt.progressSlider.value = x;
            }
        }
        if ( storage.amount > 0 && !isWorking && hasEnergy)
        {
            storage.amount--;
            if (lInt.isOpen && lInt.currentLab == this)
            {
                lInt.inputSlot.amount = storage.amount;
                lInt.inputSlot.RefreshGraphics();
            }
            isWorking = true;
            spriteRenderer.sprite = onSprite;
            thisLight.enabled = true;
            itemShower.gameObject.SetActive(true);
            itemShowerShadow.gameObject.SetActive(true);
            itemShower.sprite = storage.item.texture;
            itemShowerShadow.sprite = storage.item.texture;
            startTime = Time.time;
            Invoke(nameof(Process), storage.item.processTime);
        }
        else
        {
            if (isWorking && !hasEnergy)
            {
                isWorking = false;
                spriteRenderer.sprite = offSprite;
                thisLight.enabled = false;
                itemShower.gameObject.SetActive(false);
                itemShowerShadow.gameObject.SetActive(false);
            }
        }
    }
}
