using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBoxManager : MonoBehaviour
{
    [SerializeField] InfoBox infoBox;
    [SerializeField] GameObject infoBoxDrop;
    [SerializeField] Camera mainCamera;
    [SerializeField] Inventory inventory;
    [SerializeField] Transform blockSelection;
    [SerializeField] Color normalBlockSelectionColor;
    [SerializeField] Color farBlockSelectionColor;
    [SerializeField] SpriteRenderer blockSelectionRenderer;
    [SerializeField] ToolInteract toolInteract;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] Tools tools;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask dropLayer;
    [SerializeField] LaserAmmoManager laserAmmoManager;
    [SerializeField] GameObject infoBoxCrafting;
    [SerializeField] GameObject infoBoxOre;
    [SerializeField] GameObject infoBoxBuilding;
    bool wasActive;
    bool wasSelectionActive;

    private void Update()
    {
        if (inventory.isMouseHovering || laserAmmoManager.isHovering)
        {
            SetNotActive();
            SetNotActiveBlock();
            return;
        }

        RaycastHit2D hit = toolInteract.ObjectRaycast();
        if (hit.collider == null)
        {
            SetNotActive();
            SetNotActiveBlock();
            return;
        }

        if (hit.collider.gameObject.CompareTag("tile"))
            UpdateTile(hit.collider.gameObject);
        else if (hit.collider.gameObject.CompareTag("building"))
            UpdateBuilding(hit.collider.gameObject);
        else
            SetNotActiveBlock();
        if (hit.collider.gameObject.TryGetComponent(out InfoData data))
            UpdateInfoData(data, hit.collider.gameObject);
    }

    void UpdateTile(GameObject tile)
    {
        if (!wasSelectionActive)
        {
            blockSelection.gameObject.SetActive(true);
            wasSelectionActive = true;
        }
        blockSelection.position = tile.transform.position;
        if (Vector3.Distance(toolInteract.transform.position, tile.transform.position) > toolInteract.mineDistance)
            blockSelectionRenderer.color = farBlockSelectionColor;
        else
        {
            blockSelectionRenderer.color = normalBlockSelectionColor;
            if ((tile.layer == 6 && tools.toolIndex != 0) || (tile.layer == 7 && tools.toolIndex != 1))
                blockSelectionRenderer.color = farBlockSelectionColor;
        }
        if (infoBoxBuilding.activeSelf)
            infoBoxBuilding.SetActive(false);
        
    }
    void UpdateBuilding(GameObject building)
    {
        if (Vector3.Distance(buildingManager.transform.position, building.transform.position) > buildingManager.interactDistance)
        {
            blockSelectionRenderer.color = farBlockSelectionColor;
        }
        else
        {
            blockSelectionRenderer.color = normalBlockSelectionColor;
            buildingManager.TryInteractBuilding(building);
        }

        if (!building.GetComponent<BuildingRuntime>().isPlaced)
            return;

        if (!wasSelectionActive)
        {
            blockSelection.gameObject.SetActive(true);
            wasSelectionActive = true;
        }
        blockSelection.position = building.transform.position;
        if (building.GetComponent<Building>().referenceItem.buildingData.doGetData)
        {
            infoBox.SetBuilding(building.GetComponent<Building>());
            if (!infoBoxBuilding.activeSelf)
                infoBoxBuilding.SetActive(true);
            
        }
        else if (infoBoxBuilding.activeSelf)
            infoBoxBuilding.SetActive(false);
    }

    void UpdateInfoData(InfoData data, GameObject thing)
    {
        infoBox.Set(data);
        if (!wasActive)
        {
            wasActive = true;
            infoBox.gameObject.SetActive(true);
        }
        if (data.data == null)
        {
            if (infoBoxDrop.activeSelf)
                infoBoxDrop.SetActive(false);
            return;
        }

        if (data.data.onDropItem != null)
        {
            infoBox.SetDrop(data.data);
            if (!infoBoxDrop.activeSelf)
                infoBoxDrop.SetActive(true);
        }
        else
        {
            if (infoBoxDrop.activeSelf)
                infoBoxDrop.SetActive(false);
        }
        if (thing.TryGetComponent(out Ore ore))
        {
            infoBox.SetOre(ore);
            if (!infoBoxOre.activeSelf)
                infoBoxOre.SetActive(true);
        }
        else
        {
            if (infoBoxOre.activeSelf)
                infoBoxOre.SetActive(false);
        }
    }

    void SetNotActive()
    {
        if (!wasActive) return;
        
        infoBox.gameObject.SetActive(false);
        infoBoxDrop.SetActive(false);
        infoBoxOre.SetActive(false);
        infoBoxCrafting.SetActive(false);
        infoBoxBuilding.SetActive(false);
        wasActive = false;
    }

    void SetNotActiveBlock()
    {
        if (!wasSelectionActive) return;
        
        wasSelectionActive = false;
        blockSelection.gameObject.SetActive(false);
        infoBoxBuilding.SetActive(false);
    }

    public void SetFromItem(Item item,int amount = 0)
    {
        infoBox.gameObject.SetActive(true);
        infoBox.Set(item,amount);
        wasActive=true;
    }

    public void SetFromCraftingInfo(Item item,string message)
    {
        infoBoxCrafting.SetActive(true);
        infoBox.SetCrafting(item,message);
    }
}
