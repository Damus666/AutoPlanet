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
    public float attackSpeed;
    public float health;
    public float vision;
    public float playerDamage;
    public float buildingDamage;

    public bool canFly;
    public Sprite sprite;
    public Item dropItem;
    public int dropAmount;
    public int dropChance;
}
