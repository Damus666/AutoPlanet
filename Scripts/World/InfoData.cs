using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoData : MonoBehaviour
{
    public string visualName;
    public string description;
    public Sprite texture;
    public WorldObjectData data = null;

    public void Set(WorldObjectData data)  
    {
        visualName = data.visualName;
        description = data.description;
        texture = data.texture;
        this.data = data;
    }

    public void Set(Item data)
    {
        visualName = data.itemName;
        description = data.description;
        texture = data.texture;
    }

    public void Set(EnemyData data)
    {
        visualName = data.enemyName;
        description = data.description;
        texture = data.sprite;
    }
}
