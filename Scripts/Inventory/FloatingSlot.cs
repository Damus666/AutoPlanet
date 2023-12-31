using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI amountTxt;

    public Item item;
    public int amount;
    public Slot originalSlot;
    public bool isFloating;

    private void Update()
    {
        transform.position = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.Q) && originalSlot!=null)
        {
            if (originalSlot.isEmpty)
            {
                originalSlot.FromFloatingSlot();
                NoFloatNoMore();
            } else
            {
                Inventory.i.AddItem(item, amount);
                NoFloatNoMore();
            }
        }
    }

    public void Set(Slot slot=null, int amount=1, Item iitem=null)
    {
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
        originalSlot = slot;
        if (slot == null)
        {
            item = iitem;
        }
        else
        {
            item = slot.item;
        }
        this.amount = amount;
        image.sprite = item.texture;
        amountTxt.text = amount.ToString();
        isFloating = true;
        BuildingManager.i.OnSlotChange();
    }

    public void RefreshText()
    {
        amountTxt.text = amount.ToString();
    }

    public void RefreshImage()
    {
        image.sprite = item.texture;
    }

    public void NoFloatNoMore()
    {
        gameObject.SetActive(false);
        isFloating = false;
        BuildingManager.i.OnSlotChange();
    }

    public void RefreshBuildingManager()
    {
        BuildingManager.i.OnSlotChange();
    }
}
