using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrafterSelection : MonoBehaviour
{
    [SerializeField] Image image;

    Item selectedItem;
    CrafterInterface cInt;

    bool isHovering;

    public void POINTERENTER()
    {
        isHovering = true;
    }

    public void POINTEREXIT()
    {
        isHovering = false;
    }

    private void Update()
    {
        if (!isHovering) return;

        InfoBoxManager.i.SetFromItem(selectedItem);
        InfoBoxManager.i.SetFromCraftingInfo(selectedItem, CraftStatus.CanCraft, "Using crafter");
    }

    public void Setup(CrafterInterface c,Item i)
    {
        cInt = c;
        selectedItem = i;
        image.sprite = selectedItem.texture;
    }

    public void ONCLICK()
    {
        cInt.ONITEMSELECT(selectedItem);
    }
}
