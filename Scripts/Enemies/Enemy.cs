using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Base,
    Spitter,
    Beast
}

public enum EnemyStatus
{
    Wonder,
    Attack
}

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] Transform thrusterParticles;
    [SerializeField] float direction;
    [SerializeField] float dirdir = 1;
    [SerializeField] float searchCooldown = 1;
    [SerializeField] float distFromPlayer = 50;
    [SerializeField] bool wantToStop = false;
    
    SpriteRenderer spriteRenderer;
    Transform attackTarget;
    Transform playerT;

    InfoData infoData;
    EnemySpawner spawner;
    AudioSource hitSound;

    Vector3 normalScale;
    Vector3 facingLeftScale;
    Vector3 animationOffset = Vector3.zero;

    float lastSearch;
    float lastAttack;
    float maxHomeDistance = 10;
    float maxAnimationY = 0.4f;
    float animSpeed = 1;
    float y;
    float yDir;
    int animationDirection = 1;

    bool canAttack;
    bool canAttackInternal = true;
    bool dead = false;
    string baseDescription;

    public EnemyType enemyType = EnemyType.Base;
    public EnemyData enemyData;
    public EnemyStatus status = EnemyStatus.Wonder;
    public float health;
    public bool canSearchOtherTargets;

    public void Setup(EnemyData enemyData,EnemySpawner spawner,Transform p)
    {
        this.enemyData = enemyData;
        this.spawner = spawner;
        playerT = p;
        infoData = GetComponent<InfoData>();
        infoData.Set(enemyData);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemyData.sprite;
        health = enemyData.health;
        hitSound = GameObject.Find("Player").GetComponent<Player>().hitSound;
        normalScale = transform.localScale;
        facingLeftScale = new Vector3(-normalScale.x,normalScale.y,1);
        if (Random.Range(0, 100) < 50)
        {
            dirdir = -1;
            transform.localScale = facingLeftScale;
            thrusterParticles.localScale = normalScale;
        }
        y = transform.position.y;
        baseDescription = infoData.description;
        RefreshDescription();
    }

    private void OnDestroy()
    {
        try
        {
            spawner.children.Remove(this);
        } catch { }
    }

    public void Damage(float amount)
    {
        health -= amount;
        bloodParticles.Play();
        hitSound.Play();
        RefreshDescription();
        if (health <= 0)
        {
            Die();
        }
    }

    void RefreshDescription()
    {
        infoData.description = $"{baseDescription}\nHealth: {(int)health}/{enemyData.health}";
    }

    void Die()
    {
        bloodParticles.Play();
        spriteRenderer.color = new Color(0, 0, 0, 0);
        canAttack = false;
        dead = true;
        if (Random.Range(0,100) < enemyData.dropChance)
            playerT.GetComponent<Inventory>().DropMultiple(enemyData.dropItem, enemyData.dropAmount, transform.position);
        Destroy(thrusterParticles.gameObject);
        Destroy(GetComponent<CircleCollider2D>());
        Destroy(gameObject,1);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) { return; }
        float pdist = Vector3.Distance(transform.position, playerT.position);
        if (pdist <= distFromPlayer)
        {
            animationOffset.y += Time.deltaTime * animSpeed * animationDirection;
            if (Mathf.Abs(animationOffset.y) > maxAnimationY)
            {
                animationOffset.y = maxAnimationY * animationDirection;
                animationDirection *= -1;
            }
        }
        float speed = enemyData.speed;
        float time = Time.time;
        float deltaTime = Time.deltaTime;
        float myX = transform.position.x;
        float myY = transform.position.y;
        if (status == EnemyStatus.Wonder && pdist <= distFromPlayer)
        {
            speed *= 0.8f;
            // choose to stop
            if ((!wantToStop || direction == 0) && Random.Range(0f, 1f) < 0.001)
            {
                wantToStop = !wantToStop;
            }
            // stop
            if (wantToStop)
            {
                direction += -Mathf.Sign(direction - 0) * deltaTime * speed;
                if (Mathf.Abs(direction) < 0.02f)
                {
                    direction = 0;
                }
            } else
            {
                // change direction
                direction = Mathf.Lerp(direction, dirdir, deltaTime * speed * 2);
                if (Random.Range(0f, 100f) < 0.08f)
                {
                    dirdir *= -1;
                    if (dirdir > 0)
                    {
                        transform.localScale = normalScale;
                        thrusterParticles.localScale = facingLeftScale;
                    }
                    else
                    {
                        transform.localScale = facingLeftScale;
                        thrusterParticles.localScale = normalScale;
                    }
                }
                if (Random.Range(0, 100) < 10)
                {
                    yDir = 1;
                } else if (Random.Range(0, 100) < 10)
                {
                    yDir = -1;
                } else
                {
                    yDir = 0;
                }
                y += yDir * deltaTime * speed * 5;
                if (spawner != null)
                {
                    float dist = spawner.transform.position.y - y;
                    if (Mathf.Abs(dist) > maxHomeDistance * 0.5f)
                    {
                        yDir = Mathf.Sign(dist);
                    }
                }
            }
            //move
            if (spawner != null)
            {
                float dist = spawner.transform.position.x - myX;
                if (Mathf.Abs(dist) > maxHomeDistance)
                {
                    wantToStop = false;
                    float old = dirdir;
                    dirdir = Mathf.Sign(dist);
                    if (dirdir != old)
                    {
                        if (dirdir > 0)
                        {
                            transform.localScale = normalScale;
                            thrusterParticles.localScale = facingLeftScale;
                        }
                        else
                        {
                            transform.localScale = facingLeftScale;
                            thrusterParticles.localScale = normalScale;
                        }
                    }
                }
            }

            transform.position = new Vector3(
                Mathf.Lerp(myX, myX + direction, deltaTime * speed),
                Mathf.Lerp(myY, y + animationOffset.y, deltaTime * speed),
                0); ;

            // check attacks
            if (time - lastSearch > searchCooldown)
            {
                RaycastHit2D[] hitsInfo = Physics2D.CircleCastAll(transform.position, enemyData.vision, Vector2.zero);
                foreach (RaycastHit2D hitinfo in hitsInfo)
                {
                    if (hitinfo.collider.gameObject.layer == 3 || hitinfo.collider.gameObject.CompareTag("building"))
                    {
                        if (canAttackInternal)
                        {
                            SetStatus(EnemyStatus.Attack);
                            attackTarget = hitinfo.collider.gameObject.transform;
                        }
                    }
                }
                lastSearch = time;
            }

        } else if (status == EnemyStatus.Attack)
        {
            //speed *= 0.8f;
            float xdir = attackTarget.transform.position.x - myX;
            float ydir = Mathf.Sign(attackTarget.transform.position.y - myY);
            float old = direction;
            direction = Mathf.Sign(xdir);
            if (Mathf.Sign(old) != Mathf.Sign(direction)) {
                if (direction > 0)
                {
                    transform.localScale = normalScale;
                    thrusterParticles.localScale = facingLeftScale;
                }
                else
                {
                    transform.localScale = facingLeftScale;
                    thrusterParticles.localScale = normalScale;
                }
            }
            //horizontalMovement = direction*speed * Time.deltaTime;
            transform.position = new Vector3(
                Mathf.Lerp(myX, myX + direction, deltaTime * speed)
                , Mathf.Lerp(myY, myY + ydir + animationOffset.y * 4, deltaTime * speed)
                , 0);
            if (!canSearchOtherTargets)
            {
                if (time - lastSearch > searchCooldown)
                {
                    bool foundOne = false;
                    RaycastHit2D[] hitsInfo = Physics2D.CircleCastAll(transform.position, enemyData.vision, Vector2.zero);
                    foreach (RaycastHit2D hitinfo in hitsInfo)
                    {
                        if (hitinfo.collider.gameObject.layer == 3 || hitinfo.collider.gameObject.CompareTag("building"))
                        {
                            foundOne = true;
                        }
                    }
                    if (!foundOne)
                    {
                        SetStatus(EnemyStatus.Wonder);
                        y = transform.position.y;
                    }
                    lastSearch = time;
                }
                
        
            } else
            {
                if (time - lastSearch > searchCooldown)
                {
                    RaycastHit2D[] hitsInfo = Physics2D.CircleCastAll(transform.position, enemyData.vision * 1.5f, Vector2.zero);
                    foreach (RaycastHit2D hitinfo in hitsInfo)
                    {
                        if (hitinfo.collider.gameObject.layer == 3 || hitinfo.collider.gameObject.CompareTag("building"))
                        {
                            if (canAttackInternal)
                            {
                                Attack(hitinfo.collider.gameObject.transform);
                            }
                        }
                    }
                    lastSearch = time;
                }
            }
        }
        if (time-lastAttack > enemyData.attackSpeed)
        {
            canAttack = true;
        }
    }

    public void SetStatus(EnemyStatus status)
    {
        this.status = status;
    }

    public void Attack(Transform target,bool canSearchOthers=false)
    {
        if (canAttackInternal)
        {
            if (status != EnemyStatus.Attack)
            {
                SetStatus(EnemyStatus.Attack);
            }
            attackTarget = target;
        }
        canSearchOtherTargets = canSearchOthers;
    }

    public void TRIGGERENTER(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("building"))
        {
            if (status == EnemyStatus.Attack && canAttack && canAttackInternal)
            {
                lastAttack = Time.time;
                canAttack = false;
                Building building = collision.gameObject.GetComponent<Building>();
                bool died = building.Damage(enemyData.buildingDamage);
                if (died)
                {
                    attackTarget = null;
                    SetStatus(EnemyStatus.Wonder);
                }
            }
        } else if (collision.gameObject.layer == 3)
        {
            if (status == EnemyStatus.Attack && canAttack && canAttackInternal)
            {
                lastAttack = Time.time;
                canAttack = false;
                Player p = collision.gameObject.GetComponent<Player>();
                p.Damage(enemyData.playerDamage, true, true);
            }
        }
    }

    public void TRIGGERSTAY(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("building"))
        {
            if (status == EnemyStatus.Attack && canAttack)
            {
                lastAttack = Time.time;
                canAttack = false;
                Building building = collision.gameObject.GetComponent<Building>();
                building.Damage(enemyData.buildingDamage);
            }
        }
        else if (collision.gameObject.layer == 3)
        {
            if (status == EnemyStatus.Attack && canAttack)
            {
                lastAttack = Time.time;
                canAttack = false;
                Player p = collision.gameObject.GetComponent<Player>();
                p.Damage(enemyData.playerDamage, true, true);
            }
        }
    }
}
