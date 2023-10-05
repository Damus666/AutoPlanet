using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CrafterInterface : Interface
{
    [SerializeField] public Slider progressSlider;
    [SerializeField] GameObject itemSelectionHolder;
    [SerializeField] Transform actualHolder;
    [SerializeField] GameObject normalHolder;
    [SerializeField] GameObject itemSelectionPrefab;
    [SerializeField] Image itemSelectedImage;
    [SerializeField] HoverOutlineEffect itemSelectedEffect;
    [SerializeField] public List<Slot> inputSlots = new();
    [SerializeField] public Slot outputSlot;
    [SerializeField] List<TMP_InputField> inputFields = new();
    [SerializeField] TMP_InputField outputField;
    [SerializeField] Sprite emptyImage;
    [SerializeField] AllItems allItemsHolder;
    public Crafter currentCrafter;
    [SerializeField] bool isSelecting;
    System.Func<TMP_InputField, bool> checkCondition;

    private void Awake()
    {
        checkCondition = new(CheckCondition);
        foreach (Item item in allItemsHolder.items)
        {
            if (item.carftType == CraftType.OnlyCrafter || item.carftType == CraftType.HandsAndCrafter)
            {
                GameObject newI = Instantiate(itemSelectionPrefab, actualHolder);
                newI.GetComponent<CrafterSelection>().Setup(this, item);
            }
        }
        
        foreach (Slot inslot in inputSlots)
        {
            inslot.slotContentChanged.AddListener(new UnityAction(INPUTSLOTCHANGE));
        }
        outputSlot.slotContentChanged.AddListener(new UnityAction(OUTPUTSLOTCHANGE));
        gameObject.SetActive(false);
    }

    public void INPUTSLOTCHANGE()
    {
        currentCrafter.InputChange();
    }

    public void OUTPUTSLOTCHANGE()
    {
        currentCrafter.OutputChange();
    }

    public void OnOpen(Crafter crafter)
    {
        currentCrafter = crafter;
        if (currentCrafter.selectedItem != null) {
            itemSelectedImage.sprite = currentCrafter.selectedItem.texture;
        } else
        {
            itemSelectedImage.sprite = emptyImage;
        }
        int i = 0;
        foreach (Checkpoint c in currentCrafter.checkpoints)
        {
            if (i <= 2)
            {
                inputFields[i].text = c.checkpointID;
            } else
            {
                outputField.text = c.checkpointID;
            }
            i++;
        }
        int o = 0;
        foreach (InternalSlot ins in currentCrafter.storages)
        {
            inputSlots[o].item = ins.item;
            inputSlots[o].amount = ins.amount;
            inputSlots[o].RefreshGraphics();
            o++;
        }
        outputSlot.item = currentCrafter.output.item;
        outputSlot.amount = currentCrafter.output.amount;
        outputSlot.RefreshGraphics();
        if (!currentCrafter.isWorking)
        {
            progressSlider.value = 0;
        }
    }

    bool CheckCondition(TMP_InputField input)
    {
        return input.isFocused;
    }

    public void SELECTNEWITEM()
    {
        itemSelectedEffect.POINTEREXIT();
        isSelecting = true;
        itemSelectionHolder.SetActive(true);
        normalHolder.SetActive(false);
    }

    public void ONITEMSELECT(Item item)
    {
        isSelecting = false;
        normalHolder.SetActive(true);
        itemSelectionHolder.SetActive(false);
        currentCrafter.selectedItem = item;
        if (item != null)
        {
            itemSelectedImage.sprite = item.texture;
        } else
        {
            itemSelectedImage.sprite = constants.emptySprite;
        }
    }

    public void INC1CHANGE(string text)
    {
        currentCrafter.checkpoints[0].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }

    public void INC2CHANGE(string text)
    {
        currentCrafter.checkpoints[1].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }

    public void INC3CHANGE(string text)
    {
        currentCrafter.checkpoints[2].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }

    public void OUTCCHANGE(string text)
    {
        currentCrafter.checkpoints[3].checkpointID = text;
        constants.onCheckpointChange.Invoke();
    }

    protected override void OnClose()
    {
        foreach (Slot slot in inputSlots)
        {
            slot.POINTEREXIT();
        }
        outputSlot.POINTEREXIT();
        itemSelectedEffect.POINTEREXIT();
        if (isSelecting)
        {
            ONITEMSELECT(null);
        }
    }

    public override bool CanClose()
    {
        return !outputField.isFocused && !inputFields.Any(checkCondition);
    }
}
