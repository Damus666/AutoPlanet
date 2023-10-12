using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory i;

    [SerializeField] GameObject inventoryInterface;
    [SerializeField] GameObject otherInterfaces;
    [SerializeField] GameObject unlockInterface;
    [SerializeField] GameObject dropPrefab;

    [SerializeField] CreateSlots slotCreator;
    [SerializeField] PetManager petManager;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] AllItems allItems;
    [SerializeField] StateManager stateManager;

    [SerializeField] Interface craftingInterface;
    [SerializeField] Interface unlockI;
    [SerializeField] Slot bodySlot;
    [SerializeField] Slot petSlot;

    [SerializeField] AudioSource clickSound;
    [SerializeField] Camera mainCamera;
    
    UnlockManager unlockManager;
    RectTransform rectT;
    RectTransform rectTUnlock;
    List<Slot> slots = new();

    public FloatingSlot floatingSlot;
    public bool isMouseHovering;

    public void SaveData(SavePlayer data)
    {
        foreach (Slot slot in slots)
        {
            if (slot.isEmpty) continue;
            data.inventorySlots.Add(new SaveSlot(slot));
        }
        data.bodySlot = new SaveSlot(bodySlot);
        data.petSlot = new SaveSlot(petSlot);
    }

    public void LoadData(SavePlayer data, List<SaveDrop> drops)
    {
        Clear();
        if (!data.bodySlot.isEmpty)
        {
            bodySlot.SetItem(SaveManager.i.GetItemFromID(data.bodySlot.itemID), data.bodySlot.amount);
        }
        if (!data.petSlot.isEmpty)
        {
            petSlot.SetItem(SaveManager.i.GetItemFromID(data.petSlot.itemID), data.petSlot.amount);
        }
        foreach (SaveSlot slot in data.inventorySlots)
        {
            AddItem(SaveManager.i.GetItemFromID(slot.itemID), slot.amount);
        }
        foreach (SaveDrop drop in drops)
        {
            SpawnDrop(SaveManager.i.GetItemFromID(drop.itemID), drop.position);
        }
        SpecialSlotsChange();
    }

    private void Awake()
    {
        i = this;
        unlockManager = GetComponent<UnlockManager>();
        rectT = inventoryInterface.GetComponent<RectTransform>();
        rectTUnlock = unlockInterface.GetComponent<RectTransform>();
        slotCreator.Init();
        Close();
        if (PlayerPrefs.GetInt("TEMPaddAllItemsStart", 0) == 1)
        {
            foreach (Item testItem in allItems.items)
            {
                if (testItem.stackSize != 1)
                {
                    AddItem(testItem, 10);
                }
                else
                {
                    AddItem(testItem, 1);
                }
            }
        }
        bodySlot.slotContentChanged.AddListener(new UnityAction(SpecialSlotsChange));
        petSlot.slotContentChanged.AddListener(new UnityAction(SpecialSlotsChange));
        foreach (Item item in allItems.items)
        {
            if (item.startsUnlocked)
            {
                unlockManager.unlockedIDs.Add(item.ID);
            }
        }
    }

    public void Clear()
    {
        foreach (Slot slot in slots)
        {
            slot.SetItem(slot.item, 0);
        }
    }

    public void AddSlot(Slot slot)
    {
        slots.Add(slot);
    }

    public bool CheckRequirements(List<Requirement> requirements)
    {
        foreach (Requirement requirement in requirements)
        {
            if (CountItem(requirement.item) < requirement.amount)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckRequirement(Requirement requirement)
    {
        if (CountItem(requirement.item) < requirement.amount)
        {
            return false;
        }
        return true;
    }

    public int CountItem(Item item)
    {
        int amount = 0;
        foreach(Slot slot in slots)
        {
            if (!slot.isEmpty)
            {
                if (slot.item.ID == item.ID)
                {
                    amount += slot.amount;
                }
            }
        }
        return amount;
    }

    public void Open(Interface i = null)
    {
        if (StateManager.i.inventoryOpen)
        {
            Close();
        }
        else
        {
            if (Tools.i.isSelecting)
            {
                Tools.i.SelectTool(Tools.i.toolIndex);
            }
            else
            {
                clickSound.Play();
            }
        }
        StateManager.i.inventoryOpen = true;
        if (i==null || i.type != InterfaceType.Unlock)
        {
            inventoryInterface.SetActive(true);
            otherInterfaces.SetActive(true);
        } else
        {
            unlockInterface.SetActive(true);
        }
        if (i != null)
        {
            StateManager.i.SetInterface(i);
            i.OpenInternal();
        } else
        {

            StateManager.i.SetInterface(craftingInterface);
            craftingInterface.OpenInternal();
        }
        pauseMenu.Close();
    }

    public void Close()
    {
        if (stateManager.currentInterface == null || stateManager.currentInterface.CanClose())
        {
            clickSound.Play();
            stateManager.inventoryOpen = false;
            inventoryInterface.SetActive(false);
            otherInterfaces.SetActive(false);
            if (stateManager.currentInterface != null)
            {
                stateManager.currentInterface.CloseInternal();
                if (stateManager.currentInterface.type == InterfaceType.Unlock)
                {
                    unlockInterface.SetActive(false);
                }
            }
            stateManager.SetInterface(null);
            isMouseHovering = false;
            foreach (Slot slot in slots)
            {
                slot.POINTEREXIT();
            }
            foreach (UnlockNodeHolder btn in unlockManager.unlockBtns)
            {
                btn.POINTEREXIT();
            }
            petSlot.POINTEREXIT();
            bodySlot.POINTEREXIT();
        }
    }

    void SpecialSlotsChange()
    {
        Player.i.SpecialStatsChange(bodySlot);
        petManager.OnPetSlotChange(petSlot);
    }

    public void SpawnDrop(Item item, Vector3 position)
    {
        if (item == null) { return; }
        GameObject newDrop = Instantiate(dropPrefab, position, Quaternion.identity);
        newDrop.GetComponent<Drop>().Setup(item);
        if (item.buildingData.buildingScale > 1)
        {
            newDrop.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            newDrop.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.45f);
        } else if (item.halfScaleDrop)
        {
            newDrop.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            newDrop.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.36f);
        }
    }

    public void DropMultiple(Item item, int amount, Vector3 position)
    {
        for (int i = 0; i < amount; i++)
            SpawnDrop(item, position);
    }

    public void DropMultiple(InternalSlot slot, Vector3 position)
    {
        for (int i = 0; i < slot.amount; i++)
            SpawnDrop(slot.item, position);
    }

    public void DropSlots(List<InternalSlot> slots, Vector3 position)
    {
        foreach (InternalSlot slot in slots)
            DropMultiple(slot, position);
    }

    public int AddItem(Item item, int amount)
    {
        int toStore = amount;
        foreach (Slot slot in slots)
        {
            if (!slot.isEmpty && !slot.isFull)
            {
                if (slot.item.ID == item.ID)
                {
                    if (floatingSlot.originalSlot != slot || !floatingSlot.isFloating)
                    {
                        int remaining = slot.AddAmount(toStore);
                        if (remaining == 0)
                        {
                            if (UFNS.i)
                                UFNS.i.SpawnInvNotif(item, amount, InvNotifType.Add);
                            return 0;
                        }
                        toStore = remaining;
                    }
                }
            }
        }
        foreach (Slot slot in slots)
        {
            if (slot.isEmpty)
            {
                if (floatingSlot.originalSlot != slot || !floatingSlot.isFloating)
                {
                    slot.SetItem(item, 0);
                    int remaining = slot.AddAmount(toStore);
                    slot.RefreshGraphics();
                    if (remaining == 0)
                    {
                        if (UFNS.i)
                            UFNS.i.SpawnInvNotif(item, amount, InvNotifType.Add);
                        return 0;
                    }
                    toStore = remaining;
                }
            }
        }
        if (UFNS.i)
            UFNS.i.SpawnInvNotif(item, amount - toStore, InvNotifType.Add);
        return toStore;
    }

    public int RemoveItem(Item item, int amount)
    {
        int toRemove = amount;
        foreach (Slot slot in slots)
        {
            if (!slot.isEmpty)
            {
                if (slot.item.ID == item.ID)
                {
                    int remaining = slot.RemoveAmount(toRemove);
                    if (remaining == 0)
                    {
                        UFNS.i.SpawnInvNotif(item, amount, InvNotifType.Remove);
                        return 0;
                    }
                    toRemove = remaining;
                }
            }
        }
        UFNS.i.SpawnInvNotif(item, amount-toRemove, InvNotifType.Remove);
        return toRemove;
    }

    private void Update()
    {
        Vector2 localMousePosition = rectT.InverseTransformPoint(Input.mousePosition);
        if (rectT.rect.Contains(localMousePosition) && StateManager.i.inventoryOpen)
        {
            isMouseHovering = true;
        } else if (StateManager.i.inventoryOpen && StateManager.i.currentInterface != null && StateManager.i.currentInterface.type == InterfaceType.Unlock && rectTUnlock.rect.Contains(localMousePosition))
        {
            isMouseHovering = true;
        } else
        {
            isMouseHovering = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (StateManager.i.inventoryOpen)
            {
                Close();
            } else
            {
                Open();
            }
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StateManager.i.inventoryOpen)
            {
                Close();
            }
        } 
        if (floatingSlot.isFloating)
        {
            if (!isMouseHovering)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    UFNS.i.SpawnInvNotif(floatingSlot.item, 1, InvNotifType.Drop);
                    floatingSlot.amount--;
                    floatingSlot.RefreshText();
                    if (floatingSlot.amount <= 0)
                    {
                        floatingSlot.NoFloatNoMore();
                    }
                    Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    pos.z = 0;
                    SpawnDrop(floatingSlot.item, pos);
                }
            }
        }
        
    }
}
