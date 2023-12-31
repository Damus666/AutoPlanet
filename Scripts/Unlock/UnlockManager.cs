using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager i;
    [SerializeField] GameObject unlockIContent;
    [SerializeField] TextMeshProUGUI infoTxt;

    public List<int> unlockedIDs = new List<int>();
    public List<UnlockNodeHolder> unlockBtns = new();
    public int researchPoints;
    public int XP;
    public int nextLevelXP=1000;
    [SerializeField] float XPMultiplier;
    List<string> unlockedNodes = new();
    UnlockNode nextUnlockNode;
    float startUnlockTime;
    Slider unlockSlider;
    UnlockNodeHolder nodeHolder;
    public bool isResearching = false;
    Color green = Color.green;
    Color red = Color.red;

    public void SaveData(SavePlayer data)
    {
        data.unlockXP = XP;
        data.nextLevelXP = nextLevelXP;
        data.unlockPoints = researchPoints;
        data.unlockedNodes = unlockedNodes;
        data.unlockedIDs = unlockedIDs;
        data.isResearching = isResearching;
        if (isResearching) data.nextUnlockNodeName = nextUnlockNode.nodeName;
    }

    public void LoadData(SavePlayer data)
    {
        XP = data.unlockXP;
        researchPoints = data.unlockPoints;
        nextLevelXP = data.nextLevelXP;
        unlockedNodes = data.unlockedNodes;
        unlockedIDs = data.unlockedIDs;
        isResearching = data.isResearching;
        foreach (var node in unlockBtns)
        {
            if (unlockedNodes.Contains(node.nodeData.nodeName))
            {
                node.IsDone();
            }
        }
        if (isResearching)
        {
            foreach (var node in unlockBtns)
            {
                if (node.nodeData.nodeName == data.nextUnlockNodeName)
                {
                    nodeHolder = node;
                    unlockSlider = node.slider;
                    nextUnlockNode = node.nodeData;
                    startUnlockTime = Time.time;
                    node.isUnlocking = true;
                    break;
                }
            }
        }
        RefreshInfo();
    }

    public void RefreshInfo()
    {
        infoTxt.text = $"{researchPoints} Points - {XP}/{nextLevelXP} XP";
        infoTxt.color = researchPoints > 0 ? green : red;
    }

    public bool CanUnlock(UnlockNode node)
    {
        if (researchPoints >= 1)
        {
            foreach (UnlockNode n in node.nodeRequirements)
            {
                if (!unlockedNodes.Contains(n.nodeName))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void Sell(int xp)
    {
        XP += xp;
        if (XP >= nextLevelXP)
        {
            researchPoints += 1;
            XP = 0;
            nextLevelXP =(int)(nextLevelXP*XPMultiplier);
            UFNS.i.SpawnNotif("Research Points", "1 more point is available in the research");
        }
        RefreshInfo();
    }

    [ContextMenu("Add 1000 XP")]
    void Add1000XP()
    {
        Sell(1000);
    }

    private void Awake()
    {
        i = this;
        unlockBtns = new(unlockIContent.GetComponentsInChildren<UnlockNodeHolder>());
        RefreshInfo();
    }

    public void UnlockNode(UnlockNode node, Slider slider, UnlockNodeHolder holder)
    {
        if (nextUnlockNode != null)
        {
            return;
        }
        if (!unlockedNodes.Contains(node.nodeName))
        {
            if (researchPoints >= 1)
            {
                foreach (UnlockNode n in node.nodeRequirements)
                {
                    if (!unlockedNodes.Contains(n.nodeName))
                    {
                        return;
                    }
                }
                researchPoints -= 1;
                nextUnlockNode = node;
                unlockSlider = slider;
                nodeHolder = holder;
                startUnlockTime = Time.time;
                isResearching = true;
                holder.isUnlocking = true;
                UFNS.i.SpawnNotif("Research Started", $"Started Researching: {node.nodeName}");
                Invoke(nameof(FinishUnlocking), node.unlockTime);
            }
        }
        RefreshInfo();
    }

    public void FinishUnlocking()
    {
        unlockedNodes.Add(nextUnlockNode.nodeName);
        foreach (Item item in nextUnlockNode.itemsOutput)
        {
            unlockedIDs.Add(item.ID);
        }
        unlockSlider.value = unlockSlider.maxValue;
        nodeHolder.IsDone();
        nodeHolder.isUnlocking = false;
        UFNS.i.SpawnNotif("Research Completed", $"Finished Researching: {nextUnlockNode.nodeName}");
        nodeHolder = null;
        nextUnlockNode = null;
        unlockSlider = null;
        isResearching = false;
        RefreshInfo();
    }

    private void Update()
    {
        if (StateManager.i.inventoryOpen && nextUnlockNode != null)
        {
            unlockSlider.value = Time.time - startUnlockTime;
        }
    }
}
