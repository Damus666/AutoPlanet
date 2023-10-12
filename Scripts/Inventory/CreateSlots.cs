using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSlots : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] int slotAmout;

    public void Init()
    {
        for (int i = 0; i <slotAmout; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab,transform);
            Slot s = newSlot.GetComponent<Slot>();
            s.ID = i;
            Inventory.i.AddSlot(s);
        }
    }

    public void Init(StorageInterface storage)
    {
        for (int i = 0; i < slotAmout; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            Slot s = newSlot.GetComponent<Slot>();
            s.ID = i;
            storage.AddSlot(s);
        }
    }
}
