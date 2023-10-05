using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum InterfaceType
{
    Generic,
    Crafting,
    Furnace,
    Crafter,
    Storage,
    Lasergun,
    Roboclone,
    Miner,
    OxygenSource,
    OxygenDistributor,
    Block,
    Lamp,
    Breaker,
    Unlock,
    Laboratory
}

public class Interface : MonoBehaviour
{
    public InterfaceType type = InterfaceType.Generic;
    public string title;
    public Inventory inventory;
    [SerializeField] TextMeshProUGUI titleTxt;
    public bool isOpen;
    public Constants constants;

    private void Awake()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
    }

    public void Open()
    {
        inventory.Open(this);
    }

    public void OpenInternal()
    {
        gameObject.SetActive(true);
        titleTxt.text = title;
        isOpen = true;
        OnOpen();
    }

    public void CloseInternal()
    {
        gameObject.SetActive(false);
        isOpen = false;
        OnClose();
    }

    protected virtual void OnOpen()
    {

    }

    protected virtual void OnClose()
    {

    }

    public virtual bool CanClose()
    {
        return true;
    }
}
