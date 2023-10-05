using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LaserPet : Pet
{
    float timeBetweenAttack = 1.3f;
    float lastAttack;
    float alienDistance = 20;
    float damage=12;
    Transform shootPoint;

    private void Awake()
    {
        var thisLight = gameObject.AddComponent<Light2D>();
        thisLight.pointLightOuterRadius = 5;
        thisLight.intensity = 0.8f;
        thisLight.color = Color.red;

        var collider2 = gameObject.AddComponent<CircleCollider2D>();
        collider2.radius = alienDistance;
        collider2.isTrigger = true;
    }

    protected override void AfterSetup()
    {
        GameObject detail = Instantiate(constants.laserPetDetail, transform);
        detail.transform.localPosition = new Vector3(0, 0.6f, 0);
        shootPoint = detail.transform;
    }

    private void Update()
    {
        Follow();
        Animate();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            if (Time.time - lastAttack > timeBetweenAttack)
            {
                lastAttack = Time.time;
                GameObject p = Instantiate(constants.projectilePrefab, transform.position, Quaternion.identity);
                Projectile pp = p.GetComponent<Projectile>();
                Vector3 dir = (collision.transform.position - shootPoint.position).normalized;
                pp.Setup(dir, constants.laserPetBulletSprite, damage, true);
            }
        }
    }
}
