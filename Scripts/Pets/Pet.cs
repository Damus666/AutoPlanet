using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetType
{
    Light,
    Gun,
    Collector
}

public class Pet : MonoBehaviour
{
    public PetType type;
    public int specialID = 3;
    protected bool shouldFollowPlayer = true;
    protected Constants constants;
    protected Transform playerT;
    [SerializeField] protected Transform targetToFollow;
    Vector3 negOffset;
    Vector3 animationOffset = Vector3.zero;
    int animationDirection = 1;
    float maxAnimationY = 0.3f;
    float animSpeed = 1;
    Vector3 normalScale = new Vector3(0.75f, 0.75f, 0);
    Vector3 facingRightScale = new Vector3(-0.75f, 0.75f, 0);
    bool facingLeft = true;
    ParticleSystem particles;
    [SerializeField] protected bool isGoingBack;

    protected void Follow()
    {
        if (shouldFollowPlayer)
        {
            float delta = Time.deltaTime;
            Vector3 offsetToUse = playerT.localScale.x < 0 ? negOffset : constants.petPosOffset;
            Vector3 offset = playerT.position + offsetToUse + animationOffset;
            Vector3 old = transform.position;
            float speed = constants.petSpeed;
            if (isGoingBack)
            {
                speed = constants.collectorPetSpeed;
                float dist = Vector3.Distance(transform.position, playerT.position);
                if (dist <= 1.5f)
                {
                    isGoingBack = false;
                }
            }
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, offset.x, delta*speed), Mathf.Lerp(transform.position.y, offset.y, delta*constants.petSpeed), 0);
            CorrectScale(old);
        } else
        {
            if (targetToFollow != null)
            {
                float delta = Time.deltaTime;
                Vector3 offset = targetToFollow.position+animationOffset;
                Vector3 old = transform.position;
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, offset.x, delta * constants.collectorPetSpeed), Mathf.Lerp(transform.position.y, offset.y, delta * constants.petSpeed), 0);
                CorrectScale(old);
            }  else
            {
                shouldFollowPlayer = true;
                isGoingBack = true;
            }   
        }
    }

    void CorrectScale(Vector3 old)
    {
        float diff = transform.position.x - old.x;
        if (diff > 0 && facingLeft)
        {
            facingLeft = false;
            transform.localScale = facingRightScale;
            particles.transform.localScale = facingRightScale;
        }
        else if (diff < 0 && !facingLeft)
        {
            facingLeft = true;
            transform.localScale = normalScale;
            particles.transform.localScale = normalScale;
        }
    }

    protected virtual void AfterSetup()
    {

    }

    protected void Animate()
    {
        animationOffset.y += Time.deltaTime * animSpeed * animationDirection;
        if (Mathf.Abs(animationOffset.y) > maxAnimationY)
        {
            animationOffset.y = maxAnimationY * animationDirection;
            animationDirection *= -1;
        }
    }

    public void Setup(Constants constants,Transform player,int ID)
    {
        this.constants = constants;
        playerT = player;
        specialID = ID;
        negOffset = new Vector3(-constants.petPosOffset.x, constants.petPosOffset.y, 0);
        Vector3 offsetToUse = playerT.localScale.x < 0 ? negOffset : constants.petPosOffset;
        Vector3 offset = playerT.position + offsetToUse;
        transform.position = offset;
        particles = GetComponentInChildren<ParticleSystem>();
        AfterSetup();
    }

    private void Update()
    {
        Follow();
        Animate();
    }
}
