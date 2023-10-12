using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockNodeHolder : MonoBehaviour
{
    [SerializeField] public UnlockNode nodeData;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI infoTxt;
    [SerializeField] Transform outputT;
    [SerializeField] GameObject outputImagePrefab;
    [SerializeField] Slider progressSlider;
    [SerializeField] Image outlineImage;

    [Header("User Feedback"),SerializeField]
    Color canUnlockColor;
    [SerializeField] Color canUnlockHovered;
    [SerializeField] Color cannotUnlockColor;
    [SerializeField] Color doneColor;
    [SerializeField] Color isUnlockingColor;
    [SerializeField] Color isResearchingColor;
    [SerializeField] List<Image> lines=new();

    bool canUnlock = false;
    public bool isUnlocking = false;
    bool isDone = false;
    bool isHovering = false;

    public Slider slider { get { return progressSlider; } }

    public void POINTERENTER()
    {
        if (!isDone)
        {
            isHovering = true;
        }
    }

    public void POINTEREXIT()
    {
        if (!isDone)
        {
            isHovering = false;
        }
    }

    private void Awake()
    {
        nameTxt.text = nodeData.nodeName;
        infoTxt.text = $"{nodeData.unlockTime} s";
        foreach (Item item in nodeData.itemsOutput)
        {
            var newImg = Instantiate(outputImagePrefab, outputT);
            newImg.GetComponent<Image>().sprite = item.texture;
        }
        progressSlider.maxValue = nodeData.unlockTime;
    }

    public void IsDone()
    {
        isDone = true;
        outlineImage.color = doneColor;
        foreach (var line in lines)
        {
            line.color = doneColor;
        }
        //GetComponent<HoverOutlineEffect>().canChangeColor = false;
    }

    public void ONCLICK()
    {
        UnlockManager.i.UnlockNode(nodeData, progressSlider,this);
    }

    void UpdateOutline()
    {
        if (!isDone)
        {
            if (isUnlocking)
            {
                outlineImage.color = isUnlockingColor;
                return;
            }
            if (canUnlock)
            {
                if (UnlockManager.i.isResearching)
                {
                    outlineImage.color = isResearchingColor;
                    return;
                }
                outlineImage.color = isHovering ? canUnlockHovered : canUnlockColor;
            } else
            {
                outlineImage.color = cannotUnlockColor;
            }
        }
    }

    private void Update()
    {
        canUnlock = UnlockManager.i.CanUnlock(nodeData);
        UpdateOutline();
    }
}
