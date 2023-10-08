using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] CreateSlots slotCreator;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_InputField outputField;

    List<Slot> slots = new();
    public Storage currentStorage;

    private void Awake()
    {
        slotCreator.Init(this);
        gameObject.SetActive(false);
    }

    public void AddSlot(Slot slot)
    {
        slots.Add(slot);
    }

    public void OnStorageOpen(Storage storage)
    {
        currentStorage = storage;
        Empty();
        Copy();
        foreach (Checkpoint check in currentStorage.checkpoints)
        {
            if (check.type == CheckpointType.Put)
            {
                inputField.text = check.checkpointID;
            }
            else
            {
                outputField.text = check.checkpointID;
            }
        }
    }

    protected override void OnClose()
    {
        currentStorage.Empty();
        Paste();
        foreach (Slot slot in slots)
        {
            slot.POINTEREXIT();
        }
    }

    public void Empty()
    {
        foreach (Slot slot in slots)
        {
            slot.Empty();
        }
    }

    public void Copy()
    {
        int i = 0;
        foreach (Slot slot in slots)
        {
            if (currentStorage.internalSlots[i].isEmpty)
            {
                slot.Empty();
            } else
            {
                slot.SetItem(currentStorage.internalSlots[i].item, currentStorage.internalSlots[i].amount);
            }
            i++;
        }
    }

    public void Paste()
    {
        int i = 0;
        foreach (Slot slot in slots)
        {
            if (slot.isEmpty)
            {
                currentStorage.internalSlots[i].Empty();
            }
            else
            {
                currentStorage.internalSlots[i].item = slot.item;
                currentStorage.internalSlots[i].amount = slot.amount;
            }
            i++;
        }
    }

    public void INPUTCHECKPOINTCHANGE(string text)
    {
        foreach (Checkpoint check in currentStorage.checkpoints)
        {
            if (check.type == CheckpointType.Put)
            {
                check.checkpointID = text;
                constants.onCheckpointChange.Invoke();
            }
        }
    }

    public void OUTPUTCHECKPOINTCHANGE(string text)
    {
        foreach (Checkpoint check in currentStorage.checkpoints)
        {
            if (check.type == CheckpointType.Take)
            {
                check.checkpointID = text;
                constants.onCheckpointChange.Invoke();
            }
        }
    }

    public override bool CanClose()
    {
        return !inputField.isFocused && !outputField.isFocused;
    }
}
