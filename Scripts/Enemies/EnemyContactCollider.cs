using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContactCollider : MonoBehaviour
{
    [SerializeField] Enemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemy.TRIGGERENTER(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        enemy.TRIGGERSTAY(collision);
    }
}
