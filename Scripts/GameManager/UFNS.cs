using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFNS : MonoBehaviour
{
    [SerializeField] GameObject inventoryUFNSPrefab;
    [SerializeField] GameObject genericUFNSPrefab;
    [SerializeField] Transform toprightNotifsHolder;
    [SerializeField] Transform invNotifsHolder;
    List<InventoryUFNS> inventoryNotifs = new();

    public void RemoveInvUFNS(InventoryUFNS ufns)
    {
        inventoryNotifs.Remove(ufns);
    }

    public void SpawnNotif(string title,string subtitle)
    {
        GameObject newNotif = Instantiate(genericUFNSPrefab, toprightNotifsHolder);
        GenericUFNS comp = newNotif.GetComponent<GenericUFNS>();
        comp.BaseSetup(this);
        comp.Setup(title, subtitle);
    }

    public void SpawnInvNotif(Item item, int amount,string type="add")
    {
        foreach (InventoryUFNS ufn in inventoryNotifs)
        {
            if (ufn.ID == item.ID && ufn.type == type)
            {
                ufn.Add(amount);
                return;
            }
        }
        GameObject newNotif = Instantiate(inventoryUFNSPrefab, invNotifsHolder);
        InventoryUFNS comp = newNotif.GetComponent<InventoryUFNS>();
        comp.BaseSetup(this);
        comp.Setup(item, amount,type);
        inventoryNotifs.Add(comp);
    }
}
