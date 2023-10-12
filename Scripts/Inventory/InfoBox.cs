using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum CraftStatus
{
    CanCraft,
    CannotCraft
}

public class InfoBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;
    [SerializeField] Image image;

    [SerializeField] TextMeshProUGUI dropNameTxt;
    [SerializeField] Image dropImage;

    [SerializeField] TextMeshProUGUI crafingMessage;
    [SerializeField] TextMeshProUGUI crafingInfo;
    [SerializeField] List<Image> craftingImages;
    [SerializeField] List<TextMeshProUGUI> craftingAmounts;

    [SerializeField] TextMeshProUGUI oreTxt;
    [SerializeField] TextMeshProUGUI buildingTxt;

    public void Set(InfoData data)
    {
        nameTxt.text = data.visualName;
        descriptionTxt.text = data.description;
        image.sprite = data.texture;
    }

    public void SetOre(Ore ore)
    {
        oreTxt.text = "Ore Amount: " + ore.amount;
    }

    public void Set(Item item, int amount = 0)
    {
        nameTxt.text = item.itemName;
        descriptionTxt.text = item.description;
        image.sprite = item.texture;
    }

    public void SetDrop(WorldObjectData data)
    {
        dropNameTxt.text = data.onDropItem.itemName;
        dropImage.sprite = data.onDropItem.texture;
    }

    public void SetBuilding(Building building)
    {
        if (building.ShowWorkingStatus())
        {
            //red ff0000
            //green 00ff00
            //orange ff7d00
            string workingStatusColor = "00ff00";
            string energyColor = "00ff00";
            string status = "Status: Working";
            string energy = "Energy: Available";
            string healthColor = "ffffff";
            if (building.health >= building.maxHealth)
            {
                healthColor = "00ff00";
            }
            if (!building.isWorking)
            {
                status = "Status: Idle";
                workingStatusColor = "ff7d00";
            }
            if (!building.hasEnergy)
            {
                energy = "Energy: Unavailable";
                energyColor = "ff0000";
            }
            if (building.referenceItem.buildingData.needElectricity)
            {
                buildingTxt.text = $"<color #{workingStatusColor}>{status}</color>\n<color #{energyColor}>{energy}</color>\n<color #{healthColor}>Health: {building.health}/{building.maxHealth}</color>";
            } else
            {
                buildingTxt.text = $"<color #{workingStatusColor}>{status}</color>\n<color #{healthColor}>Health: {building.health}/{building.maxHealth}</color>";
            }
        } else
        {
            string energyColor = "00ff00";
            string energy = "Energy: Available";
            string healthColor = "ffffff";
            if (building.health >= building.maxHealth)
            {
                healthColor = "00ff00";
            }
            if (!building.hasEnergy)
            {
                energy = "Energy: Unavailable";
                energyColor = "ff0000";
            }
            buildingTxt.text = $"<color #{energyColor}>{energy}</color>\n<color #{healthColor}>Health: {building.health}/{building.maxHealth}</color>";
        }
    }

    public void SetCrafting(Item item, CraftStatus status, string message)
    {
        crafingMessage.text = message;
        if (status == CraftStatus.CanCraft)
        {
            crafingMessage.color = Color.green;
        } else
        {
            crafingMessage.color = Color.red;
        }
        crafingInfo.text = $"Craft time: {item.craftTime} s\nOutput: {item.craftAmount}";
        int i = 0;
        foreach (Image img in craftingImages)
        {
            img.transform.parent.gameObject.SetActive(false);
        }
        foreach (Requirement req in item.requirements)
        {
            craftingImages[i].transform.parent.gameObject.SetActive(true);
            craftingImages[i].sprite = req.item.texture;
            craftingAmounts[i].text = "x "+req.amount;
            if (Inventory.i.CheckRequirement(req))
            {
                craftingAmounts[i].color = Color.green;
            } else
            {
                craftingAmounts[i].color = Color.red;
            }
            i++;
        }
    }
}
