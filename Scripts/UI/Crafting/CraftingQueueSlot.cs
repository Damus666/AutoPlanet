using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingQueueSlot : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI amountTxt;

    CraftingQueue queue;
    public Item item;

    public void Setup(Item item, int amount, CraftingQueue queue)
    {
        bgImage.sprite = item.texture;
        image.sprite = item.texture;
        amountTxt.text = amount+"";
        this.item = item;
        this.queue = queue;
    }

    public void RefreshAmount(int amount)
    {
        amountTxt.text = amount + "";
    }

    public void UpdateProgress(float fillAmount)
    {
        image.fillAmount = fillAmount;
    }

    public void ResetProgress()
    {
        image.fillAmount = 0;
    }

    public void POINTERCLICK()
    {
        queue.OnSlotClick(item);
    }
}
