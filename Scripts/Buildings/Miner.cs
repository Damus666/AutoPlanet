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
    public Item storageItem;
    public int storageAmount=0;

    public override void BuildingDestroyed(Inventory inventory)
    {
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            inventory.Close();
        }
        inventory.DropMultiple(storageItem, storageAmount, transform.position);
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
                storageItem = currentOre.dropItem;
            }
        }
        checkpoints.Add(new Checkpoint(this, CheckpointType.Take));
        AddCheckpoints();
    }

    public override InternalSlot GetResource()
    {
        InternalSlot slot = new InternalSlot(storageItem, storageAmount);
        storageAmount = 0;
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            mInt.storageSlot.amount = storageAmount;
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
        storageAmount++;
        isWorking = false;
        bool isDone = currentOre.Mine();
        if (isDone)
        {
            if (mInt.isOpen && mInt.currentMiner == this)
            {
                mInt.inventory.Close();
            }
            GameObject.Find("Player").GetComponent<Player>().toolInteract.DisableTile(currentOre.gameObject);
            mInt.inventory.SpawnDrop(referenceItem, transform.position);
            Destroy(gameObject);
        } else
        {
            if (storageAmount >= storageItem.stackSize || !hasEnergy)
            {
                spriteRenderer.sprite = offSprite;
                thisLight.enabled = false;
            }
            if (mInt.isOpen && mInt.currentMiner == this)
            {
                mInt.storageSlot.amount = storageAmount;
                mInt.storageSlot.item = storageItem;
                mInt.storageSlot.RefreshGraphics();
            }
        }
    }


    private void Update()
    {
        if (mInt.isOpen && mInt.currentMiner == this)
        {
            if (mInt.storageSlot.amount != storageAmount)
            {
                storageAmount = mInt.storageSlot.amount;
            }
        }
        if (!isWorking)
        {
            if (currentOre != null && hasEnergy)
            {
                if (storageAmount < storageItem.stackSize)
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
