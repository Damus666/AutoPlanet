using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class LasergunInterface : Interface
{
    public LaserGun currentGun;
    public Slot inputSlot;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        inputSlot.slotContentChanged.AddListener(new UnityAction(STORAGESLOTCHANGE));
        gameObject.SetActive(false);
    }

    public void OnOpen(LaserGun gun)
    {
        currentGun = gun;
        inputSlot.item = gun.storage.item;
        inputSlot.amount = gun.storage.amount;
        inputSlot.RefreshGraphics();
        foreach (Checkpoint check in currentGun.checkpoints)
        {
            inputField.text = check.checkpointID;
        }
    }

    public void STORAGESLOTCHANGE()
    {
        currentGun?.StorageChanged();
    }

    protected override void OnClose()
    {
        inputSlot.POINTEREXIT();
    }

    public override bool CanClose()
    {
        return !inputField.isFocused;
    }

    public void INPUTCHECKPOINTCHANGE(string text)
    {
        currentGun.checkpoints[0].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }
}
