using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MenuPlayer : MonoBehaviour
{
    [SerializeField] Transform mainCamera;
    [SerializeField] CinemachineVirtualCamera cineCamera;
    [SerializeField] CanvasGroup helperTxt;

    [SerializeField] ParticleSystem jetpackParticles;
    [SerializeField] ParticleSystem smokeParticles;

    [SerializeField] AudioSource bgMusic;
    [SerializeField] AudioSource jetpackSound;

    CharacterController2D cc;

    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomMax = 20;
    [SerializeField] float zoomMin = 5;
    [SerializeField] float speed;
    [SerializeField] float helperDisappearSpeed;

    float horizontalMove = 0f;
    bool jump = false;
    bool wasjumping = false;
    bool canHelperDisappear;

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

    private void Start()
    {
        cc = GetComponent<CharacterController2D>();
        rrenderer = GetComponent<SpriteRenderer>();
        ChangeStatus(Status.Idle);
    }

    public void ONLAND()
    {
        ChangeStatus(Status.Idle);
    }

    private void Update()
    {
        // MOEVEMENT
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
        if (Input.GetButton("Jump"))
        {
            jump = true;
            if (!jetpackSound.isPlaying)
            {
                jetpackSound.Play();
            }
            if (Random.Range(0, 100) < deltaTime * 13500)
            {
                jetpackParticles.Play();

            }
            if (Random.Range(0, 100) < deltaTime * 16500)
            {
                smokeParticles.Play();
            }
            if (status == Status.Idle)
            {
                ChangeStatus(Status.Jumping);
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
        }
        else
        {
            if (horizontalMove != 0)
            {
                rrenderer.sprite = flyRunSprite;
            }
            else
            {
                rrenderer.sprite = flyIdleSprite;
            }
        }
        // HELPER
        if (helperTxt.gameObject.activeSelf && (horizontalMove != 0 || status == Status.Jumping))
            canHelperDisappear = true;
        if (canHelperDisappear)
        {
            helperTxt.alpha -= helperDisappearSpeed * Time.deltaTime;
            if (helperTxt.alpha <= 0)
            {
                helperTxt.gameObject.SetActive(false);
                canHelperDisappear = false;
            }
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
        if (jump)
        {
            if (!wasjumping)
            {
                jetpackParticles.Stop();
                cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump, false);
                wasjumping = true;
            }
            else { cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump, true); }
        }
        else
        {
            wasjumping = false;
            cc.Move(horizontalMove * Time.fixedDeltaTime, false, jump, false);
        }
        jump = false;
    }
}
