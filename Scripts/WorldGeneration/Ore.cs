using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    public Item dropItem;
    public int amount;
    public float mineTime;

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
