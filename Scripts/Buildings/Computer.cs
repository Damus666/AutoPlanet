using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Building
{
    Sprite offSprite;
    Sprite onSprite;
    UnlockManager unlockManager;

    public override void FinishInit()
    {
        needElectricityToInteract = true;
        unlockManager = GameObject.Find("Player").GetComponent<UnlockManager>();
        offSprite = spriteRenderer.sprite;
        onSprite = constants.computerOnSprite;
        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.strong;
        thisLight.enabled = false;
    }

    void SetOn()
    {
        spriteRenderer.sprite = onSprite;
        thisLight.enabled = true;
        isWorking = true;
    }

    void SetOff()
    {
        spriteRenderer.sprite = offSprite;
        thisLight.enabled = false;
        isWorking = false;
    }

    private void Update()
    {
        if (unlockManager.isResearching)
        {
            if (!isWorking)
            {
                SetOn();
            }
        } else
        {
            if (isWorking)
            {
                SetOff();
            }
        }
    }
}
