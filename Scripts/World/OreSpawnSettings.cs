using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Data/Create Ore Settings", fileName = "NewOreSettings", order = 0)]
public class OreSpawnSettings:ScriptableObject
{
    public int minHeight;
    public int maxHeight;
    public NoiseSettings noise;
    public WorldObjectData tile;
    public float mineTime;
}
