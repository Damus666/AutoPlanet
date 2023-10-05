using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] FloatingSlot floatingSlot;
    GameObject currentBuilding;
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] Constants constants;
    [SerializeField] UnlockInterface unlockInterface;
    public float interactDistance = 15;
    float lastPlace;

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
                currentBuilding = Instantiate(buildingPrefab);
                currentBuilding.GetComponent<InfoData>().Set(floatingSlot.item);
                currentBuilding.GetComponent<BuildingRuntime>().Setup(floatingSlot.item.texture,floatingSlot.item.buildingData.targetInterface == InterfaceType.OxygenSource || floatingSlot.item.buildingData.targetInterface == InterfaceType.OxygenDistributor);
                if (floatingSlot.item.buildingData.buildingScale > 1)
                {
                    currentBuilding.transform.localScale = new Vector3(floatingSlot.item.buildingData.buildingScale, floatingSlot.item.buildingData.buildingScale, 1);
                    if (floatingSlot.item.buildingData.targetInterface == InterfaceType.OxygenSource || floatingSlot.item.buildingData.targetInterface == InterfaceType.OxygenDistributor)
                    {
                        currentBuilding.GetComponent<BuildingRuntime>().energyRangeIndicator.transform.localScale = new Vector3(3.1f, 3.1f, 0);
                    }
                }
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

    void PlaceBuilding()
    {
        currentBuilding.GetComponent<BuildingRuntime>().OnPlace(floatingSlot.item.buildingData.destroyVegetation);
        // ADD BUILDING
        Item buildingData = floatingSlot.item;
        switch (buildingData.buildingData.targetInterface)
        {
            case InterfaceType.Furnace:
                var compfurnace = currentBuilding.AddComponent<Furnace>();
                compfurnace.referenceItem = buildingData;
                compfurnace.SetConstants(constants);
                compfurnace.SetInterface(constants.furnaceInterface);
                compfurnace.FinishInit();
                break;
            case InterfaceType.Miner:
                var compminer = currentBuilding.AddComponent<Miner>();
                compminer.referenceItem = buildingData;
                compminer.SetConstants(constants);
                compminer.SetInterface(constants.minerInterface);
                compminer.FinishInit();
                break;
            case InterfaceType.Crafter:
                var compcrafter = currentBuilding.AddComponent<Crafter>();
                compcrafter.referenceItem = buildingData;
                compcrafter.SetConstants(constants);
                compcrafter.SetInterface(constants.crafterInterface);
                compcrafter.FinishInit();
                break;
            case InterfaceType.Storage:
                var compstorage = currentBuilding.AddComponent<Storage>();
                compstorage.referenceItem = buildingData;
                compstorage.SetConstants(constants,false);
                compstorage.SetInterface(constants.storageInterface);
                compstorage.FinishInit();
                break;
            case InterfaceType.Lasergun:
                var compgun = currentBuilding.AddComponent<LaserGun>();
                compgun.referenceItem = buildingData;
                compgun.SetConstants(constants);
                compgun.SetInterface(constants.lasergunInterface);
                compgun.FinishInit();
                break;
            case InterfaceType.Roboclone:
                var comprobo = currentBuilding.AddComponent<Roboclone>();
                comprobo.referenceItem = buildingData;
                comprobo.SetConstants(constants);
                comprobo.SetInterface(constants.robocloneInterface);
                comprobo.FinishInit();
                break;
            case InterfaceType.OxygenSource:
                var compens = currentBuilding.AddComponent<EnergySource>();
                constants.energySources.Add(compens);
                compens.referenceItem = buildingData;
                compens.SetConstants(constants,false);
                compens.FinishInit();
                break;
            case InterfaceType.OxygenDistributor:
                var compend = currentBuilding.AddComponent<EnergyDistributor>();
                constants.energyDistributors.Add(compend);
                compend.referenceItem = buildingData;
                compend.SetConstants(constants);
                compend.FinishInit();
                break;
            case InterfaceType.Block:
                var compblock = currentBuilding.AddComponent<Building>();
                compblock.referenceItem = buildingData;
                compblock.SetConstants(constants,false);
                break;
            case InterfaceType.Lamp:
                var complamp = currentBuilding.AddComponent<Lamp>();
                complamp.referenceItem = buildingData;
                complamp.SetConstants(constants);
                complamp.FinishInit();
                break;
            case InterfaceType.Breaker:
                var compbreak = currentBuilding.AddComponent<Breaker>();
                compbreak.referenceItem = buildingData;
                compbreak.SetConstants(constants);
                compbreak.FinishInit();
                break;
            case InterfaceType.Unlock:
                var compunlock = currentBuilding.AddComponent<Computer>();
                compunlock.referenceItem = buildingData;
                compunlock.SetInterface(unlockInterface);
                compunlock.SetConstants(constants);
                compunlock.FinishInit();
                break;
            case InterfaceType.Laboratory:
                var complab = currentBuilding.AddComponent<Laboratory>();
                complab.referenceItem = buildingData;
                complab.SetConstants(constants);
                complab.SetInterface(constants.laboratoryInterface);
                complab.FinishInit();
                break;
            case InterfaceType.Generic:
                Debug.LogError("Trying to place a building with 'Generic' as interface. This will cause errors");
                break;
        }
        //
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
        foreach (EnergyDistributor dist in constants.energyDistributors)
        {
            dist.CheckEnergy();
        }
        constants.onNewBuildingEvent.Invoke();
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
                PlaceBuilding();
                lastPlace = Time.time;
            }
        }
    }
}
