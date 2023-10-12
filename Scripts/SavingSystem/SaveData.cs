using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveIntPos
{
    public int x;
    public int y;
}

[System.Serializable]
public class SaveOre
{
    public SaveIntPos pos;
    public int amount;
}

[System.Serializable]
public class SaveData
{
    public int seed;
    public SavePlayer savePlayer = new();
    public List<SaveChunk> saveChunks = new();
    public List<SaveBuilding> saveBuildings = new();
    public List<SaveDrop> saveDrops = new();
    public List<SaveIntPos> saveMinedBlocks = new();
    public List<SaveOre> saveOres = new();
}
