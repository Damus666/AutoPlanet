using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

enum Status
{
    Idle,
    Running,
    Jumping
}

public class Player : MonoBehaviour
{
    public static Player i;
    [SerializeField] Transform mainCamera;
    [SerializeField] CinemachineVirtualCamera cineCamera;
    [SerializeField] GameObject deathOverlay;
    [SerializeField] GameObject UIObject;

    [SerializeField] ParticleSystem jetpackParticles;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] ParticleSystem bloodParticles;

    [SerializeField] Slider jetpackSlider;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI posTxt;
    [SerializeField] CanvasGroup damageOverlay;
    
    [SerializeField] AudioSource bgMusic;
    [SerializeField] AudioSource gameOverSound;
    [SerializeField] AudioSource jetpackSound;
    [SerializeField] AudioSource hitSound;

    CharacterController2D cc;
    Rigidbody2D rb;
    Camera actualCam;

    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomMax=20;
    [SerializeField] float zoomMin=5;
    [SerializeField] float distanceFromCameraX;
    [SerializeField] float distanceFromCameraY;
    [SerializeField] float speed;
    [SerializeField] float breakerSpeedMul = 0.2f;
    [SerializeField] float runMultiplier = 1.5f;
    [SerializeField] float maxJumpEnergy=10;
    [SerializeField] float maxHealth = 100;
    //[SerializeField] float maxOxygen=100;
    //[SerializeField] float oxygenDecreaseSpeed = 1;
    //[SerializeField] float zeroOxygenDamage = 1;
    [SerializeField] float healthIncreaseSpeed = 0.5f;
    [SerializeField] float safeYVelocity = 50;
    [SerializeField] float damageOverlaySpeed = 0.1f;
    
    float horizontalMove = 0f;
    bool jump = false;
    bool wasjumping = false;
    float jumpEnergy;
    float health;
    //float oxygen;
    float velocityInAir;

    [Header("Animation")]
    [SerializeField] float runFrameSpeed = 0.1f;
    [SerializeField] float idleFrameSpeed = 0.2f;
    [SerializeField] List<Sprite> runSprites = new();
    [SerializeField] List<Sprite> idleSprites = new();
    [SerializeField] Sprite flyIdleSprite;
    [SerializeField] Sprite flyRunSprite;

    int frameIndex = 0;
    float lastTime = 0;
    SpriteRenderer rrenderer;
    Status status = Status.Idle;
    int currentFrameCount;
    float currentFrameSpeed;
    List<Sprite> currentSprites;

    [Header("---")]
    public bool hasVeichle = false;
    public Breaker veichle;
    public bool hasEnergy { get { return jumpEnergy > 0.05; } }
    public bool isRunning { get { return runSprites.Contains(rrenderer.sprite); } }

    public void SaveData(SavePlayer data)
    {
        data.position = transform.position;
        data.health = health;
    }

    public void LoadData(SavePlayer data)
    {
        transform.position = data.position;
        health = data.health;
    }

    public bool CanGoOffVeichle()
    {
        return cc.grounded;
    }

    void SetFreezingPosition()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void ConsumeEnergy(float amount)
    {
        jumpEnergy -= amount;
        if (jumpEnergy <= 0) jumpEnergy = 0;
        jetpackSlider.value = jumpEnergy;
    }

    public void SpecialStatsChange(Slot bodySlot)
    {
        if (! bodySlot.isEmpty)
        {
            if (bodySlot.item.specialID == Constants.i.bodyPartsIDs.x)
            {
                maxHealth = bodySlot.item.specialLevel == 0 ? Constants.i.stats1Maxes.x : Constants.i.stats2Maxes.x;
            } else
            {
                maxHealth = Constants.i.stats0Maxes.x;
            }
            if (bodySlot.item.specialID == Constants.i.bodyPartsIDs.z)
            {
                maxJumpEnergy = bodySlot.item.specialLevel == 0 ? Constants.i.stats1Maxes.z : Constants.i.stats2Maxes.z;
            }
            else
            {
                maxJumpEnergy = Constants.i.stats0Maxes.z;
            }
        } else
        {
            maxJumpEnergy = Constants.i.stats0Maxes.z;
        }
        RefreshSliders();
    }

    void RefreshSliders()
    {
        healthSlider.maxValue = maxHealth;
        jetpackSlider.maxValue = maxJumpEnergy;
    }

    private void Awake()
    {
        i = this;
        cc = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        rrenderer = GetComponent<SpriteRenderer>();
        ChangeStatus(Status.Idle);
        jumpEnergy = maxJumpEnergy;
        health = maxHealth;
        actualCam = mainCamera.GetComponent<Camera>();
        Invoke(nameof(SetFreezingPosition), 1.5f);
    }

    public void OnLand()
    {
        if (velocityInAir < 0){
            float v = Mathf.Abs(velocityInAir);
            if (v>safeYVelocity){
                Damage(v-safeYVelocity);
            }
        }
        ChangeStatus(Status.Idle);
    }

    public void Die(){
        deathOverlay.SetActive(true);
        gameObject.SetActive(false);
        bgMusic.Stop();
        gameOverSound.Play();
    }

    public void Damage(float amount,bool enableBlood=true,bool enableSound=true){
        if (enableSound)
        {
            hitSound.Play();
        }
        
        health -= amount;
        if(health <= 0) {
            health = 0;
            Die();
        }
        healthSlider.value = health;
        damageOverlay.alpha = 0.5f;
        damageOverlay.gameObject.SetActive(true);
        if (enableBlood){
        if (!bloodParticles.isPlaying){
            bloodParticles.Play();
        }
        }
    }

    private void Update()
    {
        // MOEVEMENT
        posTxt.text = $"X: {(int)transform.position.x}, Y: {(int)transform.position.y}";
        float deltaTime = Time.deltaTime;
        cineCamera.m_Lens.OrthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        if (cineCamera.m_Lens.OrthographicSize > zoomMax)
        {
            cineCamera.m_Lens.OrthographicSize = zoomMax;
        }
        if (zoomMin > cineCamera.m_Lens.OrthographicSize)
        {
            cineCamera.m_Lens.OrthographicSize = zoomMin;
        }
        horizontalMove = Input.GetAxis("Horizontal") * speed;
        //if (tools.isSelecting){horizontalMove = 0;}
        // TEMP RUN
        if (Input.GetKey(KeyCode.LeftShift))
        {
            horizontalMove *= runMultiplier;
        }
        // CHANGE STATUS RUN-IDLE
        if (horizontalMove != 0)
        {
            if (status == Status.Idle)
            {
                ChangeStatus(Status.Running);
            }
        }
        else
        {
            if (status == Status.Running)
            {
                ChangeStatus(Status.Idle);
            }
        }
        // JUMP
        if (Input.GetButton("Jump") && jumpEnergy > 0 && !StateManager.i.paused)
        {
            if (!hasVeichle || cc.grounded)
            {
                jump = true;
            }
            float toremove = deltaTime * 0.5f;
            if (hasVeichle)
            {
                toremove /= breakerSpeedMul;
            }
            jumpEnergy -= toremove;
            if (!jetpackSound.isPlaying){
                jetpackSound.Play();
            }
            if (Random.Range(0,100)<deltaTime*13500){
            jetpackParticles.Play();
            
            }
            if (Random.Range(0,100)<deltaTime*16500){
                smokeParticles.Play();
            }

            if (jumpEnergy <= 0){
                jumpEnergy = 0;
            }
            jetpackSlider.value = jumpEnergy;
            if (status == Status.Idle)
            {
                ChangeStatus(Status.Jumping);
            }
        }
        // INCREASE ENERGY
        if (cc.grounded){
            if (jumpEnergy<maxJumpEnergy){
            jumpEnergy += deltaTime;
            if (jumpEnergy > maxJumpEnergy){
                jumpEnergy = maxJumpEnergy;
            }
            jetpackSlider.value = jumpEnergy;
        }
        }
        // ANIMATION
        if (status != Status.Jumping)
        {
            if (Time.time - lastTime >= currentFrameSpeed)
            {
                lastTime = Time.time;
                frameIndex += 1;
                if (frameIndex >= currentFrameCount)
                {
                    frameIndex = 0;
                }
                rrenderer.sprite = currentSprites[frameIndex];
            }
        } else
        {
            if (horizontalMove != 0)
            {
                rrenderer.sprite = flyRunSprite;
            } else
            {
                rrenderer.sprite = flyIdleSprite;
            }
        }
        // OXYGEN AND HEALTH
        if (health < maxHealth){
            health += healthIncreaseSpeed *deltaTime;
            healthSlider.value = health;
        }
        
        if (damageOverlay.alpha > 0){
            damageOverlay.alpha -= deltaTime*damageOverlaySpeed;
            if (damageOverlay.alpha <= 0){
                damageOverlay.alpha = 0;
                damageOverlay.gameObject.SetActive(false);
            }
        }
        if (hasVeichle)
        {
            horizontalMove *= breakerSpeedMul;
        }
        // UI TOGGLE
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UIObject.SetActive(!UIObject.activeSelf);
        }
    }

    void ChangeStatus(Status status)
    {
        frameIndex = 0;
        this.status = status;
        if (status == Status.Running)
        {
            currentSprites = runSprites;
            currentFrameCount = runSprites.Count;
            currentFrameSpeed = runFrameSpeed;
        }
        else if (status == Status.Idle)
        {
            currentSprites = idleSprites;
            currentFrameCount = idleSprites.Count;
            currentFrameSpeed = idleFrameSpeed;
        }
        else
        {
            currentFrameCount = 0;
            currentFrameSpeed = 0;
        }
    }

    private void FixedUpdate()
    {
        if (jump){
            if (!wasjumping){
                jetpackParticles.Stop();
            cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump,false);
            wasjumping = true;
            } else {cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump,true);}
        }
        else
        {
            wasjumping = false;
            cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump,false);
        }
        jump = false;
        velocityInAir = rb.velocity.y;
    }
}
