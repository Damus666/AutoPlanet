using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : Building
{
    Vector3 normalScale = new Vector3(1, 1, 0);
    Vector3 flippedScale = new Vector3(-1, 1, 0);
    LasergunInterface lInt;

    float lastShoot;
    float timeBetweenShoot = 0.8f;
    float damage = 20f;

    public InternalSlot storage;
    public int ammo;
    public bool canShoot { get { return ammo > 0 && hasEnergy; } }

    public override SaveBuilding SaveData()
    {
        SaveBuilding data =  BaseSaveData();
        data.storages.Add(new SaveSlot(storage));
        data.intVar = ammo;
        return data;
    }

    public override void LoadData(SaveBuilding data, SaveManager manager)
    {
        BaseLoadData(data);
        storage = data.storages[0].ToSlot(manager);
        ammo = data.intVar;
    }

    public override void FinishInit()
    {
        storage = new(constants.ammoItem, constants.ammoStartAmount);
        ammo = constants.ammoAmount;
        lInt = constants.lasergunInterface;

        AddLight();
        thisLight.pointLightOuterRadius = LightIntensity.radius;
        thisLight.intensity = LightIntensity.medium;
        thisLight.color = Color.red;

        GameObject newCollider = Instantiate(constants.laserGunColliderPrefab, transform);
        newCollider.GetComponent<LaserTurretCollider>().Setup(this);

        checkpoints.Add(new Checkpoint(this, CheckpointType.Put));
        AddCheckpoints();
    }

    public void StorageChanged()
    {
        storage.amount = lInt.inputSlot.amount;
        if (ammo == 0 && storage.amount > 0)
        {
            ammo = constants.ammoAmount;
            storage.amount -= 1;
            if (lInt.isOpen && lInt.currentGun == this)
            {
                lInt.inputSlot.amount = storage.amount;
            }
        }
    }

    public override void OnInteract()
    {
        lInt.OnOpen(this);
    }

    public override void BuildingDestroyed(Inventory inventory)
    {
        if (lInt.isOpen && lInt.currentGun == this)
        {
            inventory.Close();
        }
        inventory.DropMultiple(storage, transform.position);
    }

    public override bool CanPutResource(Item item, string ID = "")
    {
        return item.ID == storage.item.ID && !storage.isFull;
    }

    public override int PutResource(Item item, int amount, string ID = "")
    {
        int diff = storage.AddAmount(amount);
        if (ammo == 0 && storage.amount > 0)
        {
            ammo = constants.ammoAmount;
            storage.amount -= 1;
        }
        if (lInt.isOpen && lInt.currentGun == this)
        {
            lInt.inputSlot.amount = storage.amount;
        }
        return diff;
    }

    void Shoot(GameObject target)
    {
        if (!canShoot) return;

        ammo -= 1;
        if (ammo <= 0 && !storage.isEmpty)
        {
            ammo = constants.ammoAmount;
            storage.amount -= 1;
            if (lInt.isOpen && lInt.currentGun == this)
            {
                lInt.inputSlot.amount = storage.amount;
            }
        }
        SpawnProjectile(target);
    }

    void SpawnProjectile(GameObject target)
    {
        GameObject p = Instantiate(constants.projectilePrefab, transform.position, Quaternion.identity);
        Projectile pp = p.GetComponent<Projectile>();
        Vector3 dir = (target.transform.position - transform.position).normalized;
        pp.Setup(dir, constants.laserPetBulletSprite, damage, true);

        if (dir.x > 0)
            transform.localScale = flippedScale;
        else
            transform.localScale = normalScale;
    }

    public void ONTRIGGERSTAY2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 11) return;
        if (Time.time - lastShoot < timeBetweenShoot) return;

        lastShoot = Time.time;
        Shoot(collision.gameObject);
    }
}
