using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrafterSelection : MonoBehaviour
{
    [SerializeField] Image image;
    Item selectedItem;
    CrafterInterface cInt;

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
