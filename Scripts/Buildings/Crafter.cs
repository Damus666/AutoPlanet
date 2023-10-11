using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : Building
{
    CrafterInterface cInt;
    SpriteRenderer preview;
    ParticleSystem particles;
    CrafterAnimator animator;

    Sprite offSprite;
    Sprite onSprite;
    float startTime;
    int outputAmount=1;

    public Item selectedItem;
    public List<InternalSlot> storages = new();
    public InternalSlot output = new();

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        data.storages.Add(new SaveSlot(selectedItem, 1));
        data.storages.Add(new SaveSlot(output));
        foreach (InternalSlot inputSlot in storages)
        {
            data.storages.Add(new SaveSlot(inputSlot));
        }
        return data;
    }

    public override void LoadData(SaveBuilding data, SaveManager manager)
    {
        BaseLoadData(data);
        selectedItem = manager.GetItemFromID(data.storages[0].itemID);
        output = data.storages[1].ToSlot(manager);
        for (int i= 2; i < storages.Count; i++) {
            storages[i - 2].Set(data.storages[i], manager);
        }
        if (selectedItem)
        {
            outputAmount = selectedItem.craftAmount;
        }
    }

    public override void BuildingDestroyed(Inventory inventory)
    {
        if (cInt.isOpen && cInt.currentCrafter == this)
        {
            inventory.Close();
        }
        inventory.DropSlots(storages, transform.position);
        inventory.DropMultiple(output, transform.position);
    }

    public void InputChange()
    {
        int i = 0;
        foreach (Slot slot in cInt.inputSlots)
        {
            storages[i].item = slot.item;
            storages[i].amount = slot.amount;
            i++;
        }
    }

    public void OutputChange()
    {
        output.amount = cInt.outputSlot.amount;
    }

    public override void FinishInit()
    {
        for (int i = 0; i < 3; i++)
        {
            storages.Add(new InternalSlot());
        }
        offSprite = spriteRenderer.sprite;
        onSprite = constants.crafterOnSprite;
        cInt = constants.crafterInterface;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.weak;
        thisLight.color = new Color(230f / 255f, 140f / 255f, 0, 1);
        thisLight.enabled = false;

        lineOffset = new Vector3(0, 0.85f, 0);

        GameObject newPreview = Instantiate(constants.crafterPreview, transform);
        newPreview.transform.localPosition = new Vector3(0, -0.18f, 0);
        preview = newPreview.GetComponent<SpriteRenderer>();
        newPreview.SetActive(false);

        GameObject newParticles = Instantiate(constants.crafterParticles, transform);
        newParticles.transform.localPosition = new Vector3(0, -0.25f, 0);
        particles = newParticles.GetComponent<ParticleSystem>();
        newParticles.SetActive(false);

        GameObject newAnimator = Instantiate(constants.crafterAnimator, transform);
        animator = newAnimator.GetComponent<CrafterAnimator>();
        newAnimator.SetActive(false);

        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        checkpoints.Add(new Checkpoint(this, CheckpointType.Take));
        AddCheckpoints();
    }

    void SetOff()
    {
        isWorking = false;
        spriteRenderer.sprite = offSprite;
        thisLight.enabled = false;
        particles.gameObject.SetActive(false);
        preview.gameObject.SetActive(false);
        animator.gameObject.SetActive(false);
        preview.material.SetFloat("_Progress", 0);
    }

    void SetOn()
    {
        isWorking = true;
        spriteRenderer.sprite = onSprite;
        thisLight.enabled = true;
        particles.gameObject.SetActive(true);
        animator.gameObject.SetActive(true);
        preview.gameObject.SetActive(true);
    }

    bool CheckRequirements()
    {
        foreach (Requirement req in selectedItem.requirements)
        {
            int amountOfThat = 0;
            foreach (InternalSlot slot in storages)
            {
                if (!slot.isEmpty && slot.item.ID == req.item.ID)
                {
                    amountOfThat += slot.amount;
                }
            }
            if (amountOfThat < req.amount)
            {
                return false;
            }
        }
        return true;
    }

    void RemoveRequirements()
    {
        foreach (Requirement req in selectedItem.requirements)
        {
            int removed = 0;
            foreach (InternalSlot slot in storages)
            {
                if (!slot.isEmpty && slot.item.ID == req.item.ID)
                {
                    if (slot.amount == req.amount)
                    {
                        removed = req.amount;
                        slot.Empty();
                    } else if (slot.amount > req.amount)
                    {
                        removed = req.amount;
                        slot.amount -= req.amount;
                    } else
                    {
                        removed = slot.amount;
                        slot.Empty();
                    }
                }
                if (removed >= req.amount)
                {
                    break;
                }
            }
        }
        RefreshInputsInt();
    }

    void RefreshInputsInt()
    {
        if (cInt.isOpen && cInt.currentCrafter == this)
        {
            int o = 0;
            foreach (InternalSlot ins in storages)
            {
                cInt.inputSlots[o].item = ins.item;
                cInt.inputSlots[o].amount = ins.amount;
                cInt.inputSlots[o].RefreshGraphics();
                o++;
            }
        }
    }

    public override void OnInteract()
    {
        cInt.OnOpen(this);
    }

    void Craft()
    {
        isWorking = false;
        if (selectedItem == null || !CheckRequirements())
        {
            SetOff();
        }
        output.amount += outputAmount;
        if (cInt.isOpen && cInt.currentCrafter == this)
        {
            cInt.progressSlider.value = 0;
            cInt.outputSlot.item = output.item;
            cInt.outputSlot.amount = output.amount;
            cInt.outputSlot.RefreshGraphics();
        }
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        int i = 0;
        foreach (Checkpoint check in checkpoints)
        {
            if (i < 3 && check.checkpointID == ID)
            {
                InternalSlot st = storages[i];
                return st.isEmpty || (st.item.ID == item.ID && st.amount < st.item.stackSize);
            }
            i++;
        }
        return false;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        int i = 0;
        foreach (Checkpoint check in checkpoints)
        {
            if (i < 3 && check.checkpointID == ID)
            {
                int a = PutSingleResource(item, amount, storages[i]);
                RefreshInputsInt();
                return a;
            }
            i++;
        }
        return amount;
    }

    int PutSingleResource(Item item, int amount, InternalSlot storage)
    {
        storage.item = item;
        storage.amount += amount;
        if (storage.amount <= storage.item.stackSize)
        {
            return 0;
        }
        else
        {
            int temp = storage.amount - storage.item.stackSize;
            storage.amount = storage.item.stackSize;
            return temp;
        }
    }

    public override InternalSlot GetResource()
    {
        InternalSlot slot = new InternalSlot(output.item, output.amount);
        output.Empty();
        if (cInt.isOpen && cInt.currentCrafter == this)
        {
            cInt.outputSlot.amount = output.amount;
            cInt.outputSlot.RefreshGraphics();
        }
        if (slot.isEmpty)
        {
            return null;
        }
        return slot;
    }

    private void Update()
    {
        if (!isWorking)
        {
            if (selectedItem != null && hasEnergy && (output.isEmpty || output.item == selectedItem))
            {
                if (CheckRequirements())
                {
                    SetOn();
                    RemoveRequirements();
                    output.item = selectedItem;
                    outputAmount = selectedItem.craftAmount;
                    startTime = Time.time;
                    preview.sprite = selectedItem.texture;
                    animator.StartAnimation(selectedItem.craftTime);
                    Invoke(nameof(Craft), selectedItem.craftTime);
                }
            }
        } else
        {
            float x = (Time.time - startTime) / output.item.craftTime;
            preview.material.SetFloat("_Progress", x);
            if (cInt.isOpen && cInt.currentCrafter == this)
            {
                cInt.progressSlider.value = x;
            }
        }
    }
}
