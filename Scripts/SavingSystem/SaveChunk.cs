using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveChunk
{
    public int x;
    public int y;
    public List<SaveDecoration> decorations = new();
    public List<SaveCaveDecoration> caveDecorations = new();
    public List<SaveBigDecoration> bigDecorations = new();
    public List<SaveOxygenPlant> oxygenPlants = new();
    public List<SaveDust> dusts = new();
    public SaveSpawner spawner = new();
    public SaveBigStar bigStar = new();

}
