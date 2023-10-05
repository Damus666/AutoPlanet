using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinerInterface : Interface
{
    public Slot storageSlot;
    public Miner currentMiner;
    [SerializeField] TMP_InputField outputField;

    public void OnOpen(Miner miner)
    {
        currentMiner = miner;
        storageSlot.item = miner.storageItem;
        storageSlot.amount = miner.storageAmount;
        storageSlot.RefreshGraphics();
        outputField.text = currentMiner.checkpoints[0].checkpointID;
    }

    public void OUTPUTCHECKPOINTCHANGE(string text)
    {
        currentMiner.checkpoints[0].checkpointID = text;
        constants.onCheckpointChange.Invoke();
            
    }

    protected override void OnClose()
    {
        storageSlot.POINTEREXIT();
    }

    public override bool CanClose()
    {
        return !outputField.isFocused;
    }
}
