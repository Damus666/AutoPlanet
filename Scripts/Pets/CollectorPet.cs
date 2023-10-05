using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CollectorPet : Pet
{
    float tooMuchDist = 12;

    private void Awake()
    {
        var thisLight = gameObject.AddComponent<Light2D>();
        thisLight.pointLightOuterRadius = 5;
        thisLight.intensity = 0.65f;
        thisLight.color = Color.green;

        var collider1 = gameObject.AddComponent<CircleCollider2D>();
        collider1.radius = 0.36f;
        var collider2 = gameObject.AddComponent<CircleCollider2D>();
        collider2.radius = 10;
        collider2.isTrigger = true;
    }

    protected override void AfterSetup()
    {
        GameObject detail = Instantiate(constants.collectorPetDetail, transform);
        detail.transform.localPosition = new Vector3(0, 0.55f, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && targetToFollow == null)
        {
            float dist = Vector3.Distance(transform.position, playerT.position);
            if (dist < tooMuchDist)
            {
                targetToFollow = collision.transform;
                shouldFollowPlayer = false;
            } else
            {
                targetToFollow = null;
                shouldFollowPlayer = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && targetToFollow == null)
        {
            float dist = Vector3.Distance(transform.position, playerT.position);
            if (dist < tooMuchDist)
            {
                targetToFollow = collision.transform;
                shouldFollowPlayer = false;
            }
            else
            {
                targetToFollow = null;
                shouldFollowPlayer = true;
                isGoingBack = true;
            }
        }
    }
}
