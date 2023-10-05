using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPet : Pet
{
    private void Awake()
    {
        var thisLight = gameObject.AddComponent<Light2D>();
        thisLight.pointLightOuterRadius = 12;
    }

    protected override void AfterSetup()
    {
        GameObject detail = Instantiate(constants.lightPetDetail, transform);
        detail.transform.localPosition = new Vector3(0, 0.5f, 0);
    }
}
