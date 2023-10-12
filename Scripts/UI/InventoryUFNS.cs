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
    public InvNotifType type;
    public string sign;

    protected override void AboutToDestroy()
    {
        UFNS.i.RemoveInvUFNS(this);
    }

    public void Setup(Item item,int amount, InvNotifType type)
    {
        this.type = type;
        ID = item.ID;
        this.amount = amount;
        itemImg.sprite = item.texture;
        sign = "+";
        if (type == InvNotifType.Remove || type == InvNotifType.Drop)
        {
            sign = "-";
        }
        amountTxt.text = $"{sign} {amount}";
        if (type == InvNotifType.Add)
        {
            amountTxt.color = Color.green;
        } else if(type == InvNotifType.Remove)
        {
            amountTxt.color = Color.red;
        } else if (type == InvNotifType.Drop)
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
