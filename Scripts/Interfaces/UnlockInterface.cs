using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockInterface : Interface
{
    private void Awake()
    {
        //inventory = GameObject.Find("Player").GetComponent<Inventory>();
        gameObject.SetActive(false);
    }
}
