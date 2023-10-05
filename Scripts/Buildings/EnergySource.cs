using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : Building
{
    BoxCollider2D thisBoxCollider;
    Sprite onSprite;
    Sprite offSprite;

    private void OnDestroy()
    {
        constants.energySources.Remove(this);
        constants.onBuildingDestroyEvent.Invoke();
    }

    public override void FinishInit()
    {
        offSprite = spriteRenderer.sprite;
        onSprite = constants.energySourceOnSprite;
        thisBoxCollider = GetComponent<BoxCollider2D>();
        thisBoxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<OxygenPlant>())
            {
                hasEnergy = true;
                hit.collider.GetComponent<OxygenPlant>().SetEnergySource(this);
            }
            
        }
        thisBoxCollider.enabled = true;
        if (hasEnergy)
        {
            spriteRenderer.sprite = onSprite;
            AddLight();
            thisLight.pointLightOuterRadius = LightIntensity.radius;
            thisLight.intensity = LightIntensity.strong;
            thisLight.color = Color.cyan;
        }
    }

    public override bool ShowWorkingStatus()
    {
        return false;
    }

    

    public void PlantDestroyed()
    {
        thisLight.enabled = false;
        spriteRenderer.sprite = offSprite;
        hasEnergy = false;
        foreach (EnergyDistributor dist in constants.energyDistributors)
        {
            dist.hasEnergy = false;
        }
        constants.onNewBuildingEvent.Invoke();
    }
}
