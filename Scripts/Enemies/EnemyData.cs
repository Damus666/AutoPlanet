using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Data/Create Enemy", fileName = "NewEnemy", order = 0)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    [TextArea] public string description;
    public EnemyType enemyType;
    public float speed;
    public bool canFly;
    public float attackSpeed;
    public float health;
    public float vision;
    public float playerDamage;
    public float buildingDamage;
    public Sprite sprite;
}
