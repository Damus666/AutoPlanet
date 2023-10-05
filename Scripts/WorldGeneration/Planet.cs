using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Data/Create Planet", fileName = "NewPlanet", order = 0)]
public class Planet : ScriptableObject
{
    public WorldObjectData bottomSprite;
    public WorldObjectData middleSprite;
    public WorldObjectData topSprite;
    public int middleLayerHeight;
    public float isBlockValue = 0.2f;
    public float heightMultiplier = 25;
    public float caveOffset = 10;
    public NoiseSettings surfaceNoise;
    public NoiseSettings caveNoise;
    public List<OreSpawnSettings> oresSettings;
    public float isOreValue=0.5f;
}
