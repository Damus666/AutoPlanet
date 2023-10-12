using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager i;

    [SerializeField] FloatingSlot floatingSlot;
    [SerializeField] UnlockInterface unlockInterface;

    [SerializeField] GameObject buildingPrefab;
    [SerializeField] Camera mainCamera;
    
    public float interactDistance = 15;
    GameObject currentBuilding;
    float lastPlace;

    private void Awake()
    {
        i = this;
    }

    public void LoadData(List<SaveBuilding> buildings)
    {
        foreach (SaveBuilding data in buildings)
        {
            Item buildingItem = SaveManager.i.GetItemFromID(data.itemID);
            CreateBuilding(buildingItem);
            currentBuilding.transform.position = data.position;
            Building building = PlaceBuilding(buildingItem);
            building.LoadData(data);
            currentBuilding = null;
        }
        Constants.i.onNewBuildingEvent.Invoke();
    }

    public void OnSlotChange()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }
        if (floatingSlot.isFloating)
        {
            if (floatingSlot.item.isBuilding)
            {
                CreateBuilding(floatingSlot.item);
            }
        }
    }

    void CreateBuilding(Item item)
    {
        currentBuilding = Instantiate(buildingPrefab);
        currentBuilding.GetComponent<InfoData>().Set(item);
        currentBuilding.GetComponent<BuildingRuntime>().Setup(item.texture, item.buildingData.targetInterface == InterfaceType.OxygenSource || item.buildingData.targetInterface == InterfaceType.OxygenDistributor);
        if (item.buildingData.buildingScale > 1)
        {
            currentBuilding.transform.localScale = new Vector3(item.buildingData.buildingScale, item.buildingData.buildingScale, 1);
            if (item.buildingData.targetInterface == InterfaceType.OxygenSource || item.buildingData.targetInterface == InterfaceType.OxygenDistributor)
            {
                currentBuilding.GetComponent<BuildingRuntime>().energyRangeIndicator.transform.localScale = new Vector3(3.1f, 3.1f, 0);
            }
        }
    }

    public void TryInteractBuilding(GameObject buildingObj)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildingObj.GetComponent<BuildingRuntime>().isPlaced)
            {
                buildingObj.GetComponent<Building>().Interact();
            }
        }
    }

    Building PlaceBuilding(Item item)
    {
        Item buildingData = item == null ? floatingSlot.item : item;
        currentBuilding.GetComponent<BuildingRuntime>().OnPlace(buildingData.buildingData.destroyVegetation);
        // ADD BUILDING
        switch (buildingData.buildingData.targetInterface)
        {
            case InterfaceType.Furnace:
                var compfurnace = currentBuilding.AddComponent<Furnace>();
                compfurnace.referenceItem = buildingData;
                compfurnace.PreSetup();
                compfurnace.SetInterface(Constants.i.furnaceInterface);
                compfurnace.FinishInit();
                break;
            case InterfaceType.Miner:
                var compminer = currentBuilding.AddComponent<Miner>();
                compminer.referenceItem = buildingData;
                compminer.PreSetup();
                compminer.SetInterface(Constants.i.minerInterface);
                compminer.FinishInit();
                break;
            case InterfaceType.Crafter:
                var compcrafter = currentBuilding.AddComponent<Crafter>();
                compcrafter.referenceItem = buildingData;
                compcrafter.PreSetup();
                compcrafter.SetInterface(Constants.i.crafterInterface);
                compcrafter.FinishInit();
                break;
            case InterfaceType.Storage:
                var compstorage = currentBuilding.AddComponent<Storage>();
                compstorage.referenceItem = buildingData;
                compstorage.PreSetup(false);
                compstorage.SetInterface(Constants.i.storageInterface);
                compstorage.FinishInit();
                break;
            case InterfaceType.Lasergun:
                var compgun = currentBuilding.AddComponent<LaserGun>();
                compgun.referenceItem = buildingData;
                compgun.PreSetup();
                compgun.SetInterface(Constants.i.lasergunInterface);
                compgun.FinishInit();
                break;
            case InterfaceType.Roboclone:
                var comprobo = currentBuilding.AddComponent<Roboclone>();
                comprobo.referenceItem = buildingData;
                comprobo.PreSetup();
                comprobo.SetInterface(Constants.i.robocloneInterface);
                comprobo.FinishInit();
                break;
            case InterfaceType.OxygenSource:
                var compens = currentBuilding.AddComponent<EnergySource>();
                Constants.i.energySources.Add(compens);
                compens.referenceItem = buildingData;
                compens.PreSetup(false);
                compens.FinishInit();
                break;
            case InterfaceType.OxygenDistributor:
                var compend = currentBuilding.AddComponent<EnergyDistributor>();
                Constants.i.energyDistributors.Add(compend);
                compend.referenceItem = buildingData;
                compend.PreSetup();
                compend.FinishInit();
                break;
            case InterfaceType.Block:
                var compblock = currentBuilding.AddComponent<Building>();
                compblock.referenceItem = buildingData;
                compblock.PreSetup(false);
                break;
            case InterfaceType.Lamp:
                var complamp = currentBuilding.AddComponent<Lamp>();
                complamp.referenceItem = buildingData;
                complamp.PreSetup();
                complamp.FinishInit();
                break;
            case InterfaceType.Breaker:
                var compbreak = currentBuilding.AddComponent<Breaker>();
                compbreak.referenceItem = buildingData;
                compbreak.PreSetup();
                compbreak.FinishInit();
                break;
            case InterfaceType.Unlock:
                var compunlock = currentBuilding.AddComponent<Computer>();
                compunlock.referenceItem = buildingData;
                compunlock.SetInterface(unlockInterface);
                compunlock.PreSetup(Constants.i);
                compunlock.FinishInit();
                break;
            case InterfaceType.Laboratory:
                var complab = currentBuilding.AddComponent<Laboratory>();
                complab.referenceItem = buildingData;
                complab.PreSetup();
                complab.SetInterface(Constants.i.laboratoryInterface);
                complab.FinishInit();
                break;
            case InterfaceType.Generic:
                Debug.LogError("Trying to place a building with 'Generic' as interface. This will cause errors");
                break;
        }
        //
        Building buildingComp = currentBuilding.GetComponent<Building>();
        currentBuilding = null;
        floatingSlot.amount--;
        floatingSlot.RefreshText();
        if (floatingSlot.amount <= 0)
        {
            floatingSlot.NoFloatNoMore();
        }
        else
        {
            OnSlotChange();
        }
        foreach (EnergyDistributor dist in Constants.i.energyDistributors)
        {
            dist.CheckEnergy();
        }
        if (item == null)
            Constants.i.onNewBuildingEvent.Invoke();
        return buildingComp;
    }

    private void Update()
    {
        if (currentBuilding == null) return;
        
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float roundx = Mathf.Round(pos.x);
        float roundy = Mathf.Round(pos.y);
        if (floatingSlot.item.buildingData.buildingScale == 1)
        {
            float intx = (int)pos.x;
            float inty = (int)pos.y;
            if (intx == roundx)
            {
                roundx += 0.5f * Mathf.Sign(pos.x);
            }
            else { roundx -= 0.5f * Mathf.Sign(pos.x); }
            if (inty == roundy)
            {
                roundy += 0.5f * Mathf.Sign(pos.y);
            }
            else { roundy -= 0.5f * Mathf.Sign(pos.y); }
            currentBuilding.transform.position = new Vector3(roundx, roundy, 0);
        } else
        {
            currentBuilding.transform.position = new Vector3(roundx, roundy, 0);
        }
        if (Input.GetMouseButton(0) && Time.time-lastPlace >= 0.1f)
        {
            if (currentBuilding.GetComponent<BuildingRuntime>().isValid)
            {
                PlaceBuilding(null);
                lastPlace = Time.time;
            }
        }
    }
}
