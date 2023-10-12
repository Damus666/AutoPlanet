using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LightIntensity
{
    public static float veryStrong = 1f;
    public static float strong = 0.8f;
    public static float medium = 0.52f;
    public static float weak = 0.37f;
    public static float longRadius = 8f;
    public static float radius = 6f;
}

public class Building : MonoBehaviour
{
    Interface thisInterface;
    float energyRangeDistace = 12;
    Slider healthSlider;

    protected Constants constants;
    protected LineRenderer lineRenderer;
    protected SpriteRenderer spriteRenderer;
    protected UnityEngine.Rendering.Universal.Light2D thisLight;

    public Item referenceItem;
    public bool isWorking;
    public bool hasEnergy;
    public bool needElectricityToInteract = false;
    public Vector3 lineOffset = Vector3.zero;
    public List<Checkpoint> checkpoints = new();
    public float health;
    public float maxHealth;

    protected SaveBuilding BaseSaveData()
    {
        SaveBuilding saveBuilding = new SaveBuilding
        {
            itemID = referenceItem.ID,
            needElectricityToInteract = needElectricityToInteract,
            health = health,
            position = transform.position,
        };
        foreach (Checkpoint c in checkpoints)
        {
            saveBuilding.checkpoints.Add(new SaveCheckpoint
            {
                ID = c.checkpointID,
                isPut = c.type == CheckpointType.Put
            });
        }
        return saveBuilding;
    }

    public virtual SaveBuilding SaveData()
    {
        return BaseSaveData();
    }

    protected void BaseLoadData(SaveBuilding data)
    {
        transform.position = data.position;
        needElectricityToInteract = data.needElectricityToInteract;
        health = data.health;
        RefreshHealthSliderVisibility();
        for (int i=0; i < data.checkpoints.Count; i++)
        {
            SaveCheckpoint saveCheckpoint = data.checkpoints[i];
            Checkpoint checkpoint = checkpoints[i];
            if ((saveCheckpoint.isPut && checkpoint.type == CheckpointType.Put) || 
                (!saveCheckpoint.isPut && checkpoint.type == CheckpointType.Take))
            {
                checkpoint.checkpointID = saveCheckpoint.ID;
            }
        }
        constants.onCheckpointChange.Invoke();
    } 

    public virtual void LoadData(SaveBuilding data)
    {
        BaseLoadData(data);
    }

    public bool DropSelf()
    {
        return health >= referenceItem.maxHealth;
    }

    public bool Damage(float amount)
    {
        health -= amount;
        healthSlider.value = health;
        RefreshHealthSliderVisibility();
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        if (DropSelf())
        {
            Inventory.i.SpawnDrop(referenceItem, transform.position);
        }
        BuildingDestroyed();
        Destroy(gameObject);
    }

    public void Repair(float amount)
    {
        health += amount;
        healthSlider.value = health;
        RefreshHealthSliderVisibility();
        if (health > referenceItem.maxHealth)
        {
            health = referenceItem.maxHealth;
        }
    }

    void RefreshHealthSliderVisibility()
    {
        if (health < referenceItem.maxHealth)
        {
            healthSlider.gameObject.SetActive(true);
        } else
        {
            healthSlider.gameObject.SetActive(false);
        }
    }

    public bool CanRepair()
    {
        return health < referenceItem.maxHealth;
    }

    protected void AddLight()
    {
        thisLight = gameObject.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    public void SetInterface(Interface i)
    {
        thisInterface = i;
    }

    protected void AddCheckpoints()
    {
        foreach (Checkpoint check in checkpoints)
        {
            constants.globalCheckpoints.Add(check);
        }
        constants.onCheckpointChange.Invoke();
    }

    protected void RemoveCheckpoints()
    {
        foreach (Checkpoint check in checkpoints)
        {
            if (constants.globalCheckpoints.Contains(check))
            {
                constants.globalCheckpoints.Remove(check);
            }
        }
        constants.onCheckpointChange.Invoke();
    }

    private void OnDestroy()
    {
        RemoveCheckpoints();
    }

    public void ONNEWBUILDING()
    {
        CheckEnergy();
    }

    public void ONBUILDINGDESTROY()
    {
        CheckEnergy();
    }

    public void CheckEnergy()
    {
        if (referenceItem.buildingData.needElectricity)
        {
            List<Transform> closeThings = new();
            hasEnergy = false;
            foreach (EnergySource source in constants.energySources)
            {

                if (Vector3.Distance(transform.position, source.transform.position) <= energyRangeDistace)
                {
                    closeThings.Add(source.transform);
                    if (source.hasEnergy)
                    {
                        hasEnergy = true;

                        break;
                    }
                }
            }
            if (!hasEnergy)
            {
                foreach (EnergyDistributor dist in constants.energyDistributors)
                {

                    if (Vector3.Distance(transform.position, dist.transform.position) <= energyRangeDistace)
                    {
                        closeThings.Add(dist.transform);
                        if (dist.hasEnergy)
                        {
                            hasEnergy = true;

                            break;
                        }
                    }
                }
            }
            if (closeThings.Count > 0)
            {
                List<Vector3> old = new();
                foreach (var thing in closeThings)
                {
                    old.Add(thing.position + thing.GetComponent<Building>().lineOffset);
                    old.Add(transform.position + lineOffset);
                }
                lineRenderer.enabled = true;
                lineRenderer.positionCount = closeThings.Count + 1;
                lineRenderer.SetPositions(old.ToArray());
            }
            else
            {
                lineRenderer.enabled = false;
            }
            OnEnergyChecked();
        } else
        {
            hasEnergy = true;
            lineRenderer.enabled = false;
        }
    }

    protected virtual void OnEnergyChecked()
    {

    }

    public void PreSetup(bool registerEvents=true)
    {
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = referenceItem.maxHealth;
        healthSlider = GetComponent<BuildingRuntime>().healthBar.GetComponent<Slider>();
        healthSlider.maxValue = health;
        healthSlider.value = health;
        maxHealth = health;
        constants = Constants.i;
        if (registerEvents)
        {
            this.constants.onNewBuildingEvent.AddListener(new UnityAction(ONNEWBUILDING));
            this.constants.onBuildingDestroyEvent.AddListener(new UnityAction(ONBUILDINGDESTROY));
        }
    }

    public void Interact()
    {
        if (referenceItem.buildingData.canInteract && (hasEnergy||!needElectricityToInteract))
        {
            thisInterface.Open();
            OnInteract();
        }
    }

    public virtual void OnInteract()
    {

    }

    public virtual void FinishInit()
    {

    }

    public virtual bool ShowWorkingStatus()
    {
        return true;
    }

    public virtual bool CanPutResource(Item item,string ID="")
    {
        return true;
    }

    public virtual int PutResource(Item item, int amount,string ID = "")
    {
        return 0;
    }

    public virtual InternalSlot GetResource()
    {
        return null;
    }

    public virtual void BuildingDestroyed()
    {

    }
}
