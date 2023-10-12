using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRay : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] bool isFromPlayer;
    [SerializeField] float damageCooldown = 0.2f;
    [SerializeField] LaserAmmoManager ammoManager;
    BoxCollider2D bCollider;
    float lastDamage;
    int count;

    public void Setup(float damage, bool isFromPlayer, float damageCooldown, LaserAmmoManager ammoManager)
    {
        this.damage = damage;
        this.isFromPlayer = isFromPlayer;
        this.damageCooldown = damageCooldown;
        this.ammoManager = ammoManager;
    }

    private void Awake()
    {
        bCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Time.time - lastDamage < damageCooldown) return;
        if (ammoManager.laserInAmmoAmount <= 0 ) return;
        lastDamage = Time.time;
        if (count % 4 == 0)
            ammoManager.ConsumeLaser();
        count++;
        

        RaycastHit2D[] hits = new RaycastHit2D[32];
        bCollider.Cast(new Vector2(), hits);
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider) continue;
            CheckOther(hit.collider);
        }
    }

    private bool CheckOther(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (!isFromPlayer)
            {
                player.Damage(damage);
                return true;
            }
        }
        else if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (isFromPlayer)
            {
                enemy.Damage(damage);
                return true;
            }
        }
        else if (other.gameObject.TryGetComponent(out EnemySpawner spawner))
        {
            if (isFromPlayer)
            {
                spawner.Damage(damage);
                return true;
            }
        }
        return false;
    }
}
