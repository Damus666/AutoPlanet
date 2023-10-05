using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : Building
{
    public override void FinishInit()
    {
        AddLight();
        thisLight.color = Color.white;
        thisLight.intensity = LightIntensity.veryStrong;
        thisLight.pointLightOuterRadius = LightIntensity.longRadius+2;
        if (!hasEnergy)
        {
            thisLight.enabled = false;
        }
    }

    protected override void OnEnergyChecked()
    {
        if (hasEnergy)
        {
            thisLight.enabled = true;
        } else
        {
            thisLight.enabled = false;
        }
    }

    public override bool ShowWorkingStatus()
    {
        return false;
    }
}
