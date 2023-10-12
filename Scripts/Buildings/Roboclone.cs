using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CheckpointType
{
    Take, Put
}

[System.Serializable]
public class Checkpoint
{
    public Building parentBuilding;
    public string checkpointID = string.Empty;
    public CheckpointType type;

    public Checkpoint(Building parentBuilding, CheckpointType type)
    {
        this.parentBuilding = parentBuilding;
        this.type = type;
    }
}

public class Roboclone : Building
{
    Checkpoint checkpoint1;
    Checkpoint checkpoint2;
    Checkpoint selectedCheckpoint;

    Building nextBuilding;
    ParticleSystem botParticles;
    SpriteRenderer handsRenderer;
    Sprite emptySprite;
    RobocloneInterface rbInt;

    float speed;
    float lastOrientationChange;
    float minOrientationChangeTime = 0.1f;

    Vector3 direction;
    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);
    Vector3 normalRot = new Vector3(45,90,0);
    Vector3 flipRot = new Vector3(135,90,0);

    public string botID;
    public InternalSlot storage = new();

    public override SaveBuilding SaveData()
    {
        SaveBuilding data = BaseSaveData();
        data.storages.Add(new SaveSlot(storage));
        data.strVar = botID;
        return data;
    }

    public override void LoadData(SaveBuilding data)
    {
        BaseLoadData(data);
        storage = data.storages[0].ToSlot();
        botID = data.strVar;
        CHECKPOINTCHANGE();
    }

    public override void BuildingDestroyed()
    {
        if (rbInt.isOpen && rbInt.currentBot == this)
        {
            Inventory.i.Close();
        }
        Inventory.i.DropMultiple(storage, transform.position);
    }

    public override void FinishInit()
    {
        constants.onCheckpointChange.AddListener(new UnityAction(CHECKPOINTCHANGE));
        constants.onBuildingDestroyEvent.AddListener(new UnityAction(ONBUILDINGDESTROY));
        
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        GameObject part = Instantiate(constants.botParticles, transform);
        botParticles = part.GetComponent<ParticleSystem>();
        part.transform.localPosition = constants.botParticlesOffset;

        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.weak;
        thisLight.color = Color.magenta;

        GameObject hands = Instantiate(constants.itemShowerPrefab, transform);
        hands.transform.localPosition = constants.robocloneHandsOffset;
        handsRenderer = hands.GetComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder = 15;
        emptySprite = constants.emptySprite;
        lineRenderer.enabled = false;
        rbInt = constants.robocloneInterface;
        speed = constants.robocloneSpeed;
    }

    void UpdateHandsGraphics()
    {
        if (!storage.isEmpty)
        {
            handsRenderer.sprite = storage.item.texture;
        } else
        {
            handsRenderer.sprite = emptySprite;
        }
    }

    public override void OnInteract()
    {
        constants.robocloneInterface.BotOpen(this);
    }

    public new void ONBUILDINGDESTROY()
    {
        CHECKPOINTCHANGE();
        CheckEnergy();
    }

    public void CHECKPOINTCHANGE()
    {
        checkpoint1 = null;
        checkpoint2 = null;
        if (botID != string.Empty)
        {
            foreach (Checkpoint checkpoint in constants.globalCheckpoints)
            {
                if (checkpoint.checkpointID == botID)
                {
                    if (checkpoint1 == null)
                    {
                        checkpoint1 = checkpoint;
                    }
                    else
                    {
                        checkpoint2 = checkpoint;
                    }
                }
            }
            if (checkpoint1 != null && checkpoint2 != null)
            {
                if (nextBuilding != checkpoint1.parentBuilding && nextBuilding != checkpoint2.parentBuilding)
                {
                    nextBuilding = checkpoint1.parentBuilding;
                    selectedCheckpoint = checkpoint1;
                    isWorking = true;
                    CorrectDirection();
                } else
                {
                    isWorking = true;
                    CorrectDirection();
                }
            }
            else
            {
                isWorking = false;
                CorrectParticles();
            }
        } else
        {
            isWorking = false;
            CorrectParticles();
        }
    }

    void CorrectParticles()
    {
        if (isWorking)
        {
            if (!botParticles.isPlaying)
            {
                botParticles.Play();
            }
        } else
        {
            if (botParticles.isPlaying)
            {
                botParticles.Stop();
            }
        }
    }

    void CorrectDirection()
    {
        direction = nextBuilding.transform.position- transform.position;
        direction.Normalize();
        if (Time.time - lastOrientationChange >= minOrientationChangeTime)
        {
            if (direction.x > 0)
            {
                transform.localScale = flipScale;
                var shape = botParticles.shape;
                shape.rotation = flipRot;
            }
            else
            {
                transform.localScale = normalScale;
                var shape = botParticles.shape;
                shape.rotation = normalRot;
            }
            lastOrientationChange = Time.time;
        }
        CorrectParticles();
    }

    void Next()
    {
        if (selectedCheckpoint == checkpoint1)
        {
            selectedCheckpoint = checkpoint2;
        } else
        {
            selectedCheckpoint = checkpoint1;
        }
        nextBuilding = selectedCheckpoint.parentBuilding;
        CorrectDirection();
    }

    void Communicate()
    {
        if (selectedCheckpoint.type == CheckpointType.Put)
        {
            if (!storage.isEmpty)
            {
                if (nextBuilding.CanPutResource(storage.item,selectedCheckpoint.checkpointID))
                {
                    storage.amount = nextBuilding.PutResource(storage.item, storage.amount,selectedCheckpoint.checkpointID);
                }
            }
        } else
        {
            InternalSlot temp = nextBuilding.GetResource();
            if (temp != null)
            {
                storage = temp;
            }
        }
        UpdateHandsGraphics();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    { 
        if (collision.gameObject.GetComponent<Building>() == null || 
            nextBuilding == null || 
            collision.gameObject != nextBuilding.gameObject) 
            return;
        
        Communicate();
        Next();        
    }

    private void Update()
    {
        if (isWorking)
        {
            transform.position += speed * Time.deltaTime * direction;
        }
    }
}
