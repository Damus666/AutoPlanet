using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInteract : MonoBehaviour
{
    public static ToolInteract i;
    [SerializeField] Camera mainCamera;

    [SerializeField] public Color deadTileColor;
    [SerializeField] float mineTime = 0.5f;
    [SerializeField] float animSpeedMul = 10;
    [SerializeField] float playerDamage = 15;
    [SerializeField] float swordDamage = 22;
    [SerializeField] public float mineDistance = 15;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask dropLayer;

    [SerializeField] List<Sprite> sprites = new();
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Sprite projectileSprite;
    [SerializeField] Transform shootPos;
    [SerializeField] Collider2D swordCollider;
    [SerializeField] GameObject rayObj;
    [SerializeField] UnityEngine.Rendering.Universal.Light2D rayLight;
    
    [SerializeField] LaserAmmoManager ammoManager;
    [SerializeField] AudioSource shootSound;
    [SerializeField] SpriteRenderer animRenderer;

    [SerializeField] Item repairItem;
    [SerializeField] float repairAmount;

    float animSpeed;
    float frameIndex;
    bool mining;
    bool miningTile;
    bool miningBlock;
    bool isMiddleClick;
    GameObject mineObject;
    float mineStartTime;
    List<GameObject> minedTiles = new();
    List<Ore> changedOres = new();

    [SerializeField] int decorationToolIdx = 1;
    [SerializeField] int tileToolIdx = 0;
    [SerializeField] int shootToolIdx = 2;
    [SerializeField] int repairToolIdx = 3;
    [SerializeField] int swordToolIdx = 4;
    [SerializeField] int rayToolIdx = 5;
    [SerializeField] int mineButton = 0;
    [SerializeField] int repairButton = 1;
    [SerializeField] int shootButton = 0;
    [SerializeField] int attackButton = 0;
    [SerializeField] int buildingMineButton = 1;
    [SerializeField] int oreDestroyButton = 2;

    private void Awake()
    {
        i = this;
        animSpeed = (1.0f / sprites.Count) * animSpeedMul;
    }

    void Update()
    {
        // SHOOT
        if (Tools.i.toolIndex == shootToolIdx && 
            Input.GetMouseButtonDown(shootButton) && 
            !Inventory.i.isMouseHovering && !Inventory.i.floatingSlot.isFloating && 
            ammoManager.laserInAmmoAmount > 0 && 
            !StateManager.i.paused)
        {
            Shoot();
        }
        // ATTACK
        if (Tools.i.toolIndex == swordToolIdx && 
            Input.GetMouseButtonDown(attackButton) && 
            !Inventory.i.isMouseHovering && 
            !Inventory.i.floatingSlot.isFloating && 
            !StateManager.i.paused)
        {
            Attack();
        }
        // RAY
        if (Tools.i.toolIndex == rayToolIdx && 
            Input.GetMouseButton(shootButton) && 
            !Inventory.i.isMouseHovering && 
            !Inventory.i.floatingSlot.isFloating && 
            !StateManager.i.paused
            && !ammoManager.isEmpty)
            UpdateRay();
        else
            DisableRay();
        // MINING
        isMiddleClick = Input.GetMouseButton(oreDestroyButton);
        if ((Input.GetMouseButton(mineButton) || Input.GetMouseButton(buildingMineButton) || isMiddleClick) 
            && !Inventory.i.isMouseHovering && !StateManager.i.paused)
        {
            if (mining && mineObject != null)
            {
                if (Vector3.Distance(transform.position, mineObject.transform.position) > mineDistance)
                {
                    StopMining();
                }
            } else
            {
                StartMining(Input.GetMouseButton(buildingMineButton) ? true : false);
            }
        } else
        {
            StopMining();
        }
        // WHEN MINING
        if (mining && mineObject != null)
        {
            // ANIMATION
            frameIndex += animSpeed * Time.deltaTime;
            if (!animRenderer.gameObject.activeSelf) animRenderer.gameObject.SetActive(true);
            if (frameIndex >= sprites.Count)
            {
                frameIndex = 0;
                animRenderer.gameObject.SetActive(false);
            }
            animRenderer.sprite = sprites[(int)frameIndex];
            // STOP MINING
            if (Vector3.Distance(transform.position, mineObject.transform.position) > mineDistance)
            {
                StopMining();
            }
            // FINISH MINING
            if (Time.time - mineStartTime >= mineTime)
            {
                FinishMining();
            }
        }
        // REPAIRING
        if (Input.GetMouseButtonDown(repairButton))
        {
            Repair();
        }
    }

    void DisableRay()
    {
        rayLight.enabled = false;
        if (rayObj.activeSelf) rayObj.SetActive(false);
    }

    void UpdateRay()
    {
        rayLight.enabled = true;
        if (!rayObj.activeSelf) rayObj.SetActive(true);
    }

    void Shoot()
    {
        shootSound.Play();
        GameObject p = Instantiate(projectilePrefab, shootPos.position, Quaternion.identity);
        Vector3 mousePos = Input.mousePosition;
        Vector3 myPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 dir = (mousePos - myPos).normalized;
        p.GetComponent<Projectile>().Setup(dir, projectileSprite, playerDamage, true);
        ammoManager.ConsumeLaser();
    }

    void Attack()
    {
        RaycastHit2D[] hits = new RaycastHit2D[30];
        swordCollider.Cast(new Vector2(), hits);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject.layer != 11) continue;
            if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                enemy.Damage(swordDamage);
        }
    }

    void StartMining(bool isBuilding)
    {
        RaycastHit2D hit = ObjectRaycast();
        StopMining();

        if (hit.collider == null) return;
        if (Vector3.Distance(transform.position, hit.collider.transform.position) > mineDistance) return;
        
        if (hit.collider.gameObject.CompareTag("tile") && !isBuilding)
        {
            if (hit.collider.isTrigger)
            {
                if (Tools.i.toolIndex != decorationToolIdx) return; 
                if (hit.collider.GetComponent<EnemySpawner>()) return;
            } else
            {
                if (Tools.i.toolIndex != tileToolIdx) return;
                miningTile = true;
            }
        }
        else if (hit.collider.gameObject.TryGetComponent(out Building building) && isBuilding)
        {
            if (Tools.i.toolIndex != tileToolIdx) return;
            if (building.referenceItem.buildingData.targetInterface == InterfaceType.Block)
            {
                miningBlock = true;
            }
        }
        else
        {
            return;
        }

        mineObject = hit.collider.gameObject;
        animRenderer.gameObject.SetActive(true);
        animRenderer.transform.position = mineObject.transform.position;
        frameIndex = 0;
        mineStartTime = Time.time;
        mining = true;
    }

    void FinishMining()
    {
        if (!mining || mineObject == null) return;

        if (miningTile || miningBlock)
        {
            DestroyThingOnTop(mineObject);
        }
        if (miningTile)
        {
            bool disabled = DisableTile(mineObject, false);
            if (disabled)
                DropFromInfoData(mineObject);
        } else
        {
            if (mineObject.CompareTag("building"))
            {
                bool canProceed = AlertBuildingDestroy(mineObject);
                if (!canProceed)
                {
                    StopMining();
                    return;
                }
            } else if (mineObject.CompareTag("tile"))
            {
                if (mineObject.GetComponent<EnemySpawner>() != null)
                {
                    StopMining();
                    return;
                }
                DropFromInfoData(mineObject);
            } else
            {
                StopMining();
                return;
            }
            Destroy(mineObject);
        }

        StopMining();
    }

    bool AlertBuildingDestroy(GameObject building)
    {
        if (building.TryGetComponent(out Breaker breaker))
        {
            if (Player.i.hasVeichle && Player.i.veichle == breaker)
            {
                return false;
            }
        }
        if (!building.GetComponent<BuildingRuntime>().isPlaced) return false;
        Building buildingComp = building.GetComponent<Building>();
        if (buildingComp.DropSelf())
        {
            Inventory.i.SpawnDrop(buildingComp.referenceItem, building.transform.position);
        }
        buildingComp.BuildingDestroyed();
        Constants.i.onBuildingDestroyEvent.Invoke();
        return true;
    }

    void Repair()
    {
        if (Tools.i.toolIndex != repairToolIdx) return;
        RaycastHit2D hit = ObjectRaycast();
        if (hit.collider == null) return;
        if (Vector3.Distance(transform.position, hit.collider.transform.position) > mineDistance) return;
        if (!hit.collider.gameObject.CompareTag("building")) return;
        if (Inventory.i.CountItem(repairItem) <= 0) return;
        if (!hit.collider.gameObject.GetComponent<BuildingRuntime>().isPlaced) return;

        Building building = hit.collider.gameObject.GetComponent<Building>();
        if (!building.CanRepair()) return;

        Inventory.i.RemoveItem(repairItem, 1);
        building.Repair(repairAmount);
    }

    void DropFromInfoData(GameObject obj)
    {
        Inventory.i.SpawnDrop(obj.GetComponent<InfoData>().data.onDropItem, obj.transform.position);
    }

    void DestroyThingOnTop(GameObject tile)
    {
        Vector3 pos = tile.transform.position + Vector3.up;
        RaycastHit2D hit = ObjectPosRaycast(pos);

        if (hit.collider == null) return;

        if (hit.collider.gameObject.CompareTag("tile"))
        {
            if ((int)hit.collider.transform.position.x == (int)tile.transform.position.x && hit.collider.isTrigger) 
            {
                DestroyDecoration(hit.collider.gameObject);
            }
        } else if (hit.collider.gameObject.CompareTag("building"))
        {
            bool canProceed = AlertBuildingDestroy(hit.collider.gameObject);
            if (canProceed) Destroy(hit.collider.gameObject);
        }
    }

    public bool DisableTile(GameObject tile, bool forceDisable)
    {
        if (!forceDisable && isMiddleClick && tile.TryGetComponent(out Ore ore))
        {
            ore.Mine();
            Inventory.i.AddItem(ore.dropItem, 1);
            if (ore.amount > 0)
                return false;
        }
        SpriteRenderer sRenderer = tile.GetComponent<SpriteRenderer>();
        BoxCollider2D bCollider = tile.GetComponent<BoxCollider2D>();
        sRenderer.color = deadTileColor;
        sRenderer.sortingOrder = 3;
        bCollider.enabled = false;
        minedTiles.Add(tile);
        return true;
    }

    public void SaveData(SaveData data)
    {
        foreach (GameObject obj in minedTiles)
        {
            data.saveMinedBlocks.Add(new SaveIntPos
            {
                x = (int)obj.transform.position.x,
                y = (int)obj.transform.position.y
            });
        }
        foreach (Ore ore in changedOres)
        {
            data.saveOres.Add(new SaveOre
            {
                pos = new SaveIntPos
                {
                    x = (int)ore.transform.position.x,
                    y = (int)ore.transform.position.y
                },
                amount = ore.amount
            });
        }
    }

    public void RegisterMinedTile(GameObject tile)
    {
        minedTiles.Add(tile);
    }

    public void RegisterChangedOre(Ore ore)
    {
        changedOres.Add(ore);
    }

    void StopMining()
    {
        mining = false;
        miningTile = false;
        miningBlock = false;
        mineObject = null;
        frameIndex = 0;
        if (animRenderer.gameObject.activeSelf) animRenderer.gameObject.SetActive(false);
    }

    public void DestroyTile(GameObject tile, bool forceDisable)
    {
        bool disabled = DisableTile(tile, forceDisable);
        if (!disabled) return;
        DropFromInfoData(tile);
        DestroyThingOnTop(tile);
    }

    public void DestroyDecoration(GameObject dec)
    {
        if (dec.GetComponent<EnemySpawner>() != null) return;

        DropFromInfoData(dec);
        Destroy(dec);
    }

    public RaycastHit2D ObjectRaycast()
    {
        return Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector3.zero, 1, ~(playerLayer | dropLayer));
    }

    RaycastHit2D ObjectPosRaycast(Vector3 pos)
    {
        return Physics2D.Raycast(pos, Vector3.zero, 1, ~(playerLayer | dropLayer));
    }
}
