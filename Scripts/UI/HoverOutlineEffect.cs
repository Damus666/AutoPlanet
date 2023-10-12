using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverOutlineEffect : MonoBehaviour
{
    [SerializeField] Color normalColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
    [SerializeField] Color hoverColor = Color.white;
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
