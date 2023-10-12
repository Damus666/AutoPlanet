using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool isFromPlayer = false;
    [SerializeField] float speed;
    [SerializeField] float destroyTime = 10;
    [SerializeField] float angleCorrection;
    [SerializeField] float allowedTileCollisions = 100;
    float collidedNum;
    float damage;

    public void Setup(Vector3 direction,Sprite sprite,float damage,bool isPlayer){
        isFromPlayer = isPlayer;
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<Rigidbody2D>().velocity = direction*speed;
        this.damage = damage;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - angleCorrection, Vector3.forward);
        Destroy(gameObject,destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent(out Player player)){
            if (!isFromPlayer){
                player.Damage(damage);
                Destroy(gameObject);
            }
        } else if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            if (isFromPlayer)
            {
                enemy.Damage(damage);
                Destroy(gameObject);
            }
        } else if (other.gameObject.TryGetComponent(out EnemySpawner spawner))
        {
            if (isFromPlayer)
            {
                spawner.Damage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer == 6){
            collidedNum++;
            if (collidedNum >= allowedTileCollisions){
                Destroy(gameObject);
            }
        }
    }

    
}
