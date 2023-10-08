using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Constants : MonoBehaviour
{
    [HideInInspector]public int seed = 0;

    public Vector3 stats0Maxes = new Vector3(100, 100, 1);
    public Vector3 stats1Maxes = new Vector3(120, 120, 1.2f);
    public Vector3 stats2Maxes = new Vector3(150, 150, 1.5f);
    public Vector3Int bodyPartsIDs = new Vector3Int();
    public Vector3Int petsIDs = new Vector3Int(3, 4, 5);
    public Vector3 petPosOffset = new Vector3(-1, 1, 0);
    
    public GameObject lightPetDetail;
    public GameObject laserPetDetail;
    public GameObject collectorPetDetail;
    public GameObject projectilePrefab;
    public Sprite laserPetBulletSprite;

    public bool enemyExpeditionActive;
    public float lastExpedition;
    public float petSpeed = 10;
    public float collectorPetSpeed = 2.5f;

    [Header("Interfaces")]
    public CrafterInterface crafterInterface;
    public CraftingInterface craftingInterface;
    public FurnaceInterface furnaceInterface;
    public MinerInterface minerInterface;
    public StorageInterface storageInterface;
    public LasergunInterface lasergunInterface;
    public RobocloneInterface robocloneInterface;
    public LaboratoryInterface laboratoryInterface;

    [Header("Buildings Related")]
    public Sprite furnaceOnSprite;
    public Sprite minerOnSprite;
    public Sprite energySourceOnSprite;
    public Sprite energyDistributorOnSprite;
    public Sprite emptySprite;
    public Sprite computerOnSprite;
    public Sprite labOnSprite;
    public Sprite crafterOnSprite;
    public List<Checkpoint> globalCheckpoints = new();
    public float robocloneSpeed;
    public GameObject botParticles;
    public GameObject itemShowerPrefab;
    public Vector3 botParticlesOffset;
    public Vector3 robocloneHandsOffset;
    public GameObject crafterPreview;
    public GameObject crafterParticles;
    public GameObject crafterAnimator;
    public Item ammoItem;
    public int ammoStartAmount;
    public int ammoAmount;
    public GameObject laserGunColliderPrefab;

    [Header("Events")]
    [HideInInspector] public UnityEvent onNewBuildingEvent;
    [HideInInspector] public UnityEvent onBuildingDestroyEvent;
    [HideInInspector] public UnityEvent onCheckpointChange;

    public List<EnergySource> energySources=new();
    public List<EnergyDistributor> energyDistributors=new();

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
    }
}
