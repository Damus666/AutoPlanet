using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    public Item dropItem;
    public int amount;
    public float mineTime;

    private void Awake()
    {
        SaveOre oreData = SaveManager.i.WasOreChanged(this);
        if (oreData != null)
            amount = oreData.amount;
        
    }

    public bool Mine()
    {
        amount--;
        if (amount <= 0)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
