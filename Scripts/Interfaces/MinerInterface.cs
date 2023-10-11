using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinerInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] TMP_InputField outputField;

    public Slot storageSlot;
    public Miner currentMiner;

    public void OnOpen(Miner miner)
    {
        currentMiner = miner;
        storageSlot.item = miner.storage.item;
        storageSlot.amount = miner.storage.amount;
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
