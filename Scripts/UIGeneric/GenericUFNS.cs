using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericUFNS : BaseUFNS
{
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI subtitleTxt;

    public void Setup(string title, string subtitle)
    {
        titleTxt.text = title;
        subtitleTxt.text = subtitle;
    }
}
