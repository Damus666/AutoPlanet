using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FurnaceInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_InputField outputField;

    public Slot inputSlot;
    public Slot outputSlot;
    public Slider progressSlider;
    public Furnace currentFurnace;
    
    public void OnOpen(Furnace furnace)
    {
        currentFurnace = furnace;
        inputSlot.item = furnace.itemSmelting;
        inputSlot.amount = furnace.amount;
        inputSlot.RefreshGraphics();
        outputSlot.item = furnace.outputItem;
        outputSlot.amount = furnace.outputAmount;
        outputSlot.RefreshGraphics();
        progressSlider.value = 0;
        foreach (Checkpoint check in currentFurnace.checkpoints)
        {
            if (check.type == CheckpointType.Put)
            {
                inputField.text = check.checkpointID;
            } else
            {
                outputField.text = check.checkpointID;
            }
        }
    }

    public void INPUTCHECKPOINTCHANGE(string text)
    {
        foreach (Checkpoint check in currentFurnace.checkpoints)
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
        foreach (Checkpoint check in currentFurnace.checkpoints)
        {
            if (check.type == CheckpointType.Take)
            {
                check.checkpointID = text;
                constants.onCheckpointChange.Invoke();
            }
        }
    }

    protected override void OnClose()
    {
        inputSlot.POINTEREXIT();
        outputSlot.POINTEREXIT();
    }

    public override bool CanClose()
    {
        return !inputField.isFocused && !outputField.isFocused;
    }
}
