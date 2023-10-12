using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LaserAmmoManager : MonoBehaviour
{
    [SerializeField] Item laserAmmoItem;
    public int laserAmmoAmount;
    public int laserInAmmoAmount;
    [SerializeField] int laserInAmmoAmountMax;
    [SerializeField] Image ammoBG;
    [SerializeField] Image ammoImg;

    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] TextMeshProUGUI laserAmountTxt;
    [SerializeField] FloatingSlot floatingSlot;
    [SerializeField] Color normalOutlineColor;
    [SerializeField] Color hoveredOutlineColor = Color.white;
    [SerializeField] Image outlineRenderer;
    [SerializeField] Slider amountSlider;
    public bool isEmpty
    {
        get
        {
            return laserAmmoAmount == 0;
        }
    }
    public bool isFull
    {
        get
        {
            return laserAmmoAmount == laserAmmoItem.stackSize;
        }
    }
    public bool isHovering;

    public void POINTERENTER()
    {
        outlineRenderer.color = hoveredOutlineColor;
        isHovering = true;
    }

    public void POINTEREXIT()
    {
        outlineRenderer.color = normalOutlineColor;
        isHovering = false;
    }

    private void Update()
    {
        if (isHovering)
        {
            InfoBoxManager.i.SetFromItem(laserAmmoItem,laserAmmoAmount);
        }
    }

    public void POINTERCLICK(BaseEventData eventData)
    {
        PointerEventData pointerData = eventData as PointerEventData;
        bool isLeft = true;
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            isLeft = false;
        }
        if (floatingSlot.isFloating)
        {
            if (isEmpty)
            {
                if (isLeft)
                {
                    if (floatingSlot.item.ID == laserAmmoItem.ID)
                    {
                        AddAmount(floatingSlot.amount);
                    }
                    floatingSlot.NoFloatNoMore();
                }
                else
                {
                    if (floatingSlot.item.ID == laserAmmoItem.ID)
                    {
                        laserAmmoAmount++;
                        RefreshText();
                        floatingSlot.amount--;
                        floatingSlot.RefreshText();
                        if (floatingSlot.amount <= 0)
                        {
                            floatingSlot.NoFloatNoMore();
                        }
                    }
                }

            }
            else
            {
                    if (floatingSlot.item.ID == laserAmmoItem.ID)
                    {
                        if (isLeft)
                        {
                            laserAmmoAmount += floatingSlot.amount;
                            floatingSlot.amount = 0;
                            int diff = laserAmmoAmount - laserAmmoItem.stackSize;
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
                                laserAmmoAmount++;
                                floatingSlot.amount--;
                                floatingSlot.RefreshText();
                                if (floatingSlot.amount <= 0)
                                {
                                    floatingSlot.NoFloatNoMore();
                                }
                            }
                        }
                        RefreshText();

                    }
                
            }
        }
        else
        {
            if (!isEmpty)
            {
                if (isLeft)
                {
                    floatingSlot.Set(null, laserAmmoAmount, laserAmmoItem);
                    Empty();
                }
                else
                {
                    if (laserAmmoAmount > 1)
                    {
                        int previus = laserAmmoAmount;
                        floatingSlot.Set(null, laserAmmoAmount / 2,laserAmmoItem);
                        laserAmmoAmount = previus - floatingSlot.amount;
                        RefreshText();
                    }
                    else
                    {
                        floatingSlot.Set(null, laserAmmoAmount,laserAmmoItem);
                        Empty();
                    }
                }
            }
        }
    }

    public void Empty()
    {
        laserAmmoAmount = 0;
        RefreshText();
    }

    private void Awake()
    {
        RefreshText();
    }

    public int AddAmount(int amount)
    {
        laserAmmoAmount += amount;
        int diff = laserAmmoAmount - laserAmmoItem.stackSize;
        if (diff > 0)
        {
            if (laserAmmoAmount > laserAmmoItem.stackSize)
            {
                laserAmmoAmount = laserAmmoItem.stackSize;
            }
            RefreshText();
            return diff;
        }
        else
        {
            RefreshText();
            return 0;
        }
    }

    public int RemoveAmount(int amount)
    {
        laserAmmoAmount -= amount;
        if (laserAmmoAmount < 0)
        {
            int diff = -laserAmmoAmount;
            laserAmmoAmount = 0;
            RefreshText();
            return diff;
        }
        else
        {
            RefreshText();
            return 0;
        }
    }

    public void ConsumeLaser()
    {
        laserInAmmoAmount--;
        if (laserInAmmoAmount <= 0)
        {
            if (laserAmmoAmount > 0)
            {
                laserAmmoAmount--;
                laserInAmmoAmount = laserInAmmoAmountMax;
            }
        }
        RefreshText();
    }

    public void RefreshText()
    {
        if (laserInAmmoAmount == 0 && laserAmmoAmount > 0)
        {
            laserAmmoAmount--;
            laserInAmmoAmount = laserInAmmoAmountMax;
        }
            amountTxt.text = "x "+laserAmmoAmount;
            laserAmountTxt.text = "(" +laserInAmmoAmount+ ")";
        amountSlider.value = laserInAmmoAmount;
        if (laserInAmmoAmount > 0)
        {
            ammoImg.gameObject.SetActive(true);
        } else
        {
            ammoImg.gameObject.SetActive(false);
        }
        
    }
}
