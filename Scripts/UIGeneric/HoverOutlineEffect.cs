using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverOutlineEffect : MonoBehaviour
{
    [SerializeField] Color normalColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Image outline;
    public bool canChangeColor = true;

    public void POINTERENTER()
    {
        if (canChangeColor)
        {
            outline.color = hoverColor;
        }
    }

    public void POINTEREXIT()
    {
        if (canChangeColor)
        {
            outline.color = normalColor;
        }
    }
}
