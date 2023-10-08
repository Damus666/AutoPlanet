using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRay : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] bool isFromPlayer;
    [SerializeField] float damageCooldown = 0.2f;
    BoxCollider2D bCollider;
    float lastDamage;

    public void Setup(float damage, bool isFromPlayer, float damageCooldown)
    {
        this.damage = damage;
        this.isFromPlayer = isFromPlayer;
        this.damageCooldown = damageCooldown;
    }

    private void Awake()
    {
        bCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Time.time - lastDamage < damageCooldown) return;
        lastDamage = Time.time;

        RaycastHit2D[] hits = new RaycastHit2D[32];
        bCollider.Cast(new Vector2(), hits);
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider) continue;
            CheckOther(hit.collider);
        }
    }

    private void CheckOther(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (!isFromPlayer)
            {
                player.Damage(damage);
            }
        }
        else if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (isFromPlayer)
            {
                print("bruh");
                enemy.Damage(damage);
            }
        }
        else if (other.gameObject.TryGetComponent(out EnemySpawner spawner))
        {
            if (isFromPlayer)
            {
                spawner.Damage(damage);
            }
        }
    }
}
