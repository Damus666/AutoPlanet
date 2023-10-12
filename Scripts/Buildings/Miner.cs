using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Building
{
    Sprite onSprite;
    Sprite offSprite;
    MinerInterface mInt;
    Transform raycastPoint;
    Ore currentOre;

    public InternalSlot storage = new();

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        data.storages.Add(new SaveSlot(storage));
        data.intVar = currentOre == null ? -1 : currentOre.amount;
        return data;
    }

    public override void LoadData(SaveBuilding data)
    {
        BaseLoadData(data);
        if (currentOre && data.intVar != -1)
            currentOre.amount = data.intVar;
        storage = data.storages[0].ToSlot();
    }

    public override void BuildingDestroyed()
    {
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            Inventory.i.Close();
        }
        Inventory.i.DropMultiple(storage, transform.position);
    }

    public override void FinishInit()
    {
        lineOffset = new Vector3(0, 0.1f, 0);
        mInt = constants.minerInterface;
        offSprite = spriteRenderer.sprite;
        onSprite = constants.minerOnSprite;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.medium;
        thisLight.color = Color.red;
        thisLight.enabled = false;
        raycastPoint = GetComponent<BuildingRuntime>().blockChecks[Random.Range(0, GetComponent<BuildingRuntime>().blockChecks.Count)];
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint.position, Vector3.zero);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Ore>())
            {
                currentOre = hit.collider.GetComponent<Ore>();
                storage.item = currentOre.dropItem;
            }
        }
        checkpoints.Add(new Checkpoint(this, CheckpointType.Take));
        AddCheckpoints();
    }

    public override InternalSlot GetResource()
    {
        InternalSlot slot = new InternalSlot(storage);
        storage.amount = 0;
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            mInt.storageSlot.amount = storage.amount;
            mInt.storageSlot.RefreshGraphics();
        }
        if (slot.isEmpty)
        {
            return null;
        }
        return slot;
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        return false;
    }

    public override void OnInteract()
    {
        mInt.OnOpen(this);
    }

    void Mine()
    {
        storage.amount++;
        isWorking = false;
        bool isDone = currentOre.Mine();
        if (isDone)
        {
            if (mInt.isOpen && mInt.currentMiner == this)
            {
                Inventory.i.Close();
            }
            ToolInteract.i.DisableTile(currentOre.gameObject, true);
            ToolInteract.i.RegisterMinedTile(currentOre.gameObject);

            Inventory.i.SpawnDrop(referenceItem, transform.position);
            Destroy(gameObject);
        } else
        {
            if (storage.amount >= storage.item.stackSize || !hasEnergy)
            {
                spriteRenderer.sprite = offSprite;
                thisLight.enabled = false;
            }
            if (mInt.isOpen && mInt.currentMiner == this)
            {
                mInt.storageSlot.amount = storage.amount;
                mInt.storageSlot.item = storage.item;
                mInt.storageSlot.RefreshGraphics();
            }
        }
    }


    private void Update()
    {
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            if (mInt.storageSlot.amount != storage.amount)
            {
                storage.amount = mInt.storageSlot.amount;
            }
        }
        if (!isWorking)
        {
            if (currentOre != null && hasEnergy)
            {
                if (storage.amount < storage.item.stackSize)
                {
                    isWorking = true;
                    spriteRenderer.sprite = onSprite;
                    thisLight.enabled = true;
                    Invoke(nameof(Mine), currentOre.mineTime);
                }
            }
        } else
        {
            thisLight.intensity = Random.Range(0.4f, 0.8f);
        }
    }
}
