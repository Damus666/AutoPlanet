using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<EnemyData> enemyDatas=new();
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject expeditionPrefab;
    [SerializeField] Slider healthSlider;
    [SerializeField] InfoData infoData;

    [SerializeField] int enemyNum;
    [SerializeField] float healthRefillSpeed;
    [SerializeField] float healthRefillCooldown;
    [SerializeField] float distanceFromPlayer;
    [SerializeField] float baseSpawnCooldown;
    [SerializeField] float criticSpawnCooldown;
    [SerializeField] float health;
    
    EnemyExpedition expedition;
    Transform player;
    
    float lastSpawn;
    float maxHealth;
    float spawnCooldown;
    float lastDamage;
    string baseDescription;
    bool canSpawn = false;

    public bool isOff;
    public List<Enemy> children = new();

    public float Health { get { return health; } }

    private void OnDestroy()
    {
        try
        {
            Destroy(expedition.gameObject);
        } catch { }
    }

    public void SetHealth(float health)
    {
        this.health = health;
        RefreshDescription();
    }

    public void Damage(float amount)
    {
        health -= amount;
        lastDamage = Time.time;
        SoundManager.i.hitSound.Play();
        RefreshDescription();
        foreach (Enemy enemy in children)
        {
            enemy.Attack(player);
        }
        if (health <= 0)
        {
            Die();
        }
    }

    void RefreshDescription()
    {
        infoData.description = $"{baseDescription}\nHealth: {(int)health}/{maxHealth}";
        healthSlider.value = health;
    }

    void Die()
    {
        List<Enemy> temp = new List<Enemy>(children);
        foreach (Enemy child in temp)
        {
            child.Damage(10000);
        }
        Destroy(gameObject);
    }

    void Awake()
    {
        player = GameObject.Find("Player").transform;
        enemyNum = Random.Range(enemyNum - 2, enemyNum + 2);
        GameObject newEx = Instantiate(expeditionPrefab, transform.position, Quaternion.identity);
        expedition = newEx.GetComponent<EnemyExpedition>();
        expedition.SetSpawner(this,player);
        healthSlider.maxValue = health;
        maxHealth = health;
        healthSlider.value = health;
        
    }

    public void FinishInit()
    {
        baseDescription = infoData.description;
        RefreshDescription();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= distanceFromPlayer)
        {
            foreach (Enemy child in children)
            {
                child.Attack(player);
            }
            spawnCooldown = criticSpawnCooldown;
        } else
        {
            spawnCooldown = baseSpawnCooldown;
        }
        if (children.Count < enemyNum && canSpawn)
        {
            SpawnEnemy();
            canSpawn = false;
            lastSpawn = Time.time;
        }
        if (!canSpawn && Time.time-lastSpawn > spawnCooldown)
        {
            canSpawn = true;
        }
        if (health < maxHealth)
        {
            if (Time.time-lastDamage > healthRefillCooldown)
            {
                health += Time.deltaTime * healthRefillSpeed;
                healthSlider.value = health;
                RefreshDescription();
            }
        }
    }

    public void SendExpedition()
    {
        foreach (Enemy child in children)
        {
            child.Attack(player, true);
        }
        children.Clear();
    }

    void SpawnEnemy()
    {
        //Vector3 offset = new Vector3(Random.Range(-3, 3), 0, 0);
        Vector3 offset = Vector3.zero;
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position + offset, Quaternion.identity);
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemy.Setup(enemyDatas[Random.Range(0, enemyDatas.Count)], this,player);
        children.Add(enemy);
    }

    public void CHUNKDISABLED()
    {
        isOff = true;
    }

    public void CHUNKENABLED()
    {
        isOff = false;
    }
}
