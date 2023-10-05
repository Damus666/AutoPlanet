using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDistributor : Building
{
    Sprite onSprite;
    Sprite offSprite;

    private void OnDestroy()
    {
        constants.energyDistributors.Remove(this);
        constants.onBuildingDestroyEvent.Invoke();
    }

    public override void FinishInit()
    {
        offSprite = spriteRenderer.sprite;
        onSprite = constants.energyDistributorOnSprite;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.weak;
        thisLight.color = Color.cyan;
        if (hasEnergy)
        {
            spriteRenderer.sprite = onSprite;
        }
        else
        {
            thisLight.enabled = false;
        }
    }

    public override bool ShowWorkingStatus()
    {
        return false;
    }

    protected override void OnEnergyChecked()
    {
        if (hasEnergy)
        {
            thisLight.enabled = true;
            spriteRenderer.sprite = onSprite;
        }
        else
        {
            spriteRenderer.sprite = offSprite;
            thisLight.enabled = false;
        }
    }
}
