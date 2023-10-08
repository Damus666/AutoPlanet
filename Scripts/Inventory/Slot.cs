using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    [SerializeField] Sprite emptyImage;
    [SerializeField] Image spriteRenderer;
    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] Color normalOutlineColor;
    [SerializeField] Color hoveredOutlineColor = Color.white;
    [SerializeField] Image outlineRenderer;

    FloatingSlot floatingSlot;
    InfoBoxManager boxManager;
    bool isHovering;

    public Item item = null;
    public int amount;
    public int ID;
    public bool canPut = true;
    public bool onlySmeltable = false;
    public bool whiteList = false;
    public List<int> allowedIDs = new List<int>();

    public UnityEvent slotContentChanged = new UnityEvent();
    public bool isEmpty { get { return amount == 0; } }
    public bool isFull { get { return amount == item.stackSize; } }

    public void POINTERENTER()
    {
        isHovering = true;
        outlineRenderer.color = hoveredOutlineColor;
    }

    public void POINTEREXIT()
    {
        isHovering=false;
        outlineRenderer.color = normalOutlineColor;
    }

    public void POINTERCLICK(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        bool isLeft = !(pointerData.button == PointerEventData.InputButton.Right);
        if (floatingSlot.isFloating)
        {
            if (canPut)
            {
                if (isEmpty)
                {
                    if (isLeft)
                    {
                        if ((floatingSlot.item.smeltedVersion != null || !onlySmeltable) && (allowedIDs.Contains(floatingSlot.item.specialID) || !whiteList))
                        {
                            FromFloatingSlot();
                            floatingSlot.NoFloatNoMore();
                            slotContentChanged.Invoke();
                        }
                    }
                    else
                    {
                        if (floatingSlot.originalSlot != this)
                        {
                            if ((floatingSlot.item.smeltedVersion != null || !onlySmeltable) && (allowedIDs.Contains(floatingSlot.item.specialID) || !whiteList))
                            {
                                SetItem(floatingSlot.item, 1);
                                floatingSlot.amount--;
                                floatingSlot.RefreshText();
                                if (floatingSlot.amount <= 0)
                                {
                                    floatingSlot.NoFloatNoMore();
                                }
                                slotContentChanged.Invoke();
                            }
                        }
                    }

                }
                else
                {
                    if (floatingSlot.originalSlot != this)
                    {
                        if (floatingSlot.item.ID == item.ID)
                        {
                            if (isLeft)
                            {
                                amount += floatingSlot.amount;
                                floatingSlot.amount = 0;
                                int diff = amount - item.stackSize;
                                if (diff > 0)
                                {
                                    floatingSlot.amount = diff;

                                }
                                else
                                {
                                    floatingSlot.NoFloatNoMore();
                                }
                                floatingSlot.RefreshText();
                            }
                            else
                            {
                                if (!isFull)
                                {
                                    amount++;
                                    floatingSlot.amount--;
                                    floatingSlot.RefreshText();
                                    if (floatingSlot.amount <= 0)
                                    {
                                        floatingSlot.NoFloatNoMore();
                                    }
                                }
                            }
                            RefreshTextOnly();
                            slotContentChanged.Invoke();
                        }
                        else
                        {
                            if ((floatingSlot.item.smeltedVersion != null || !onlySmeltable) && (allowedIDs.Contains(floatingSlot.item.specialID) || !whiteList)) {
                                Item itemTemp = item;
                                int amountTemp = amount;
                                item = floatingSlot.item;
                                amount = floatingSlot.amount;
                                floatingSlot.item = itemTemp;
                                floatingSlot.amount = amountTemp;
                                floatingSlot.RefreshText();
                                floatingSlot.RefreshImage();
                                RefreshGraphics();
                                slotContentChanged.Invoke();
                                floatingSlot.RefreshBuildingManager();
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (!isEmpty)
            {
                if (isLeft)
                {
                    floatingSlot.Set(this,amount);
                    Empty();
                } else
                {
                    if (amount > 1)
                    {
                        int previus = amount;
                        floatingSlot.Set(this, amount / 2);
                        amount = previus - floatingSlot.amount;
                        RefreshTextOnly();
                    } else
                    {
                        floatingSlot.Set(this, amount);
                        Empty();
                    }
                }
                slotContentChanged.Invoke();
            }
        }
    }

    private void Update()
    {
        if (isHovering)
        {
            if (!isEmpty)
            {
                boxManager.SetFromItem(item,amount);
            }
        }
    }
    
    public void FromFloatingSlot()
    {
        SetItem(floatingSlot.item, floatingSlot.amount);
    }

    public void Empty()
    {
        amount = 0;
        RefreshGraphics();
    }

    private void Awake()
    {
        boxManager = GameObject.Find("GameManager").GetComponent<InfoBoxManager>();
        floatingSlot = GameObject.Find("Player").GetComponent<Inventory>().floatingSlot;
        RefreshGraphics();
    }

    public int AddAmount(int amount)
    {
        this.amount += amount;
        int diff = this.amount - item.stackSize;
        if (diff > 0)
        {
            if (this.amount > item.stackSize)
            {
                this.amount = item.stackSize;
            }
            RefreshTextOnly();
            return diff;
        }else
        {
            RefreshTextOnly();
            return 0;
        }
    }

    public int RemoveAmount(int amount)
    {
        this.amount -= amount;
        if (this.amount < 0)
        {
            int diff = -this.amount;
            this.amount = 0;
            RefreshGraphics();
            return diff;
        } else
        {
            RefreshTextOnly();
            return 0;
        }
    }

    public void SetItem(Item item,int amount)
    {
        this.item = item;
        this.amount = amount;
        RefreshGraphics();
    }

    public void RefreshGraphics()
    {
        if (amount > 0)
        {
            spriteRenderer.sprite = item.texture;
            amountTxt.text = amount.ToString();
        }else
        {
            spriteRenderer.sprite = emptyImage;
            amountTxt.text = string.Empty;
        }
    }

    public void RefreshTextOnly()
    {
        if (amount > 0)
        {
            amountTxt.text = amount.ToString();
        }
        else
        {
            amountTxt.text = string.Empty;
        }
    }
}
