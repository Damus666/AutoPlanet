using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUFNS : BaseUFNS
{
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI amountTxt;
    public int amount;
    public int ID;
    public string type;
    public string sign;

    protected override void AboutToDestroy()
    {
        ufns.RemoveInvUFNS(this);
    }

    public void Setup(Item item,int amount,string type)
    {
        this.type = type;
        ID = item.ID;
        this.amount = amount;
        itemImg.sprite = item.texture;
        sign = "+";
        if (type == "remove" || type == "drop")
        {
            sign = "-";
        }
        amountTxt.text = $"{sign} {amount}";
        if (type == "add")
        {
            amountTxt.color = Color.green;
        } else if(type == "remove"){
            amountTxt.color = Color.red;
        } else if (type == "drop")
        {
            amountTxt.color = new Color(255, 200, 0, 1);
        }
    }

    public void Add(int amount)
    {
        ResetAlpha();
        this.amount += amount;
        amountTxt.text = $"{sign} {this.amount}";
        canDisappear = false;
        Invoke(nameof(SetDisappear), waitTime);
    }
}
