using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaboratoryInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] TMP_InputField inputField;

    public Slot inputSlot;
    public Slider progressSlider;
    public Laboratory currentLab;
    

    public void OnOpen(Laboratory lab)
    {
        currentLab = lab;
        inputSlot.item = lab.itemProcessing;
        inputSlot.amount = lab.amount;
        inputSlot.RefreshGraphics();
        progressSlider.value = 0;
        foreach (Checkpoint check in currentLab.checkpoints)
        {
            inputField.text = check.checkpointID;
        }
    }

    public void INPUTCHECKPOINTCHANGE(string text)
    {
        currentLab.checkpoints[0].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }

    protected override void OnClose()
    {
        inputSlot.POINTEREXIT();
    }

    public override bool CanClose()
    {
        return !inputField.isFocused;
    }
}
