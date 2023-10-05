using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurretCollider : MonoBehaviour
{
    LaserGun laserGun;

    public void Setup(LaserGun gun)
    {
        laserGun = gun;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        laserGun.ONTRIGGERSTAY2D(collision);
    }
}
