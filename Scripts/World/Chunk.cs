using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    [SerializeField] Planet planetData;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int chunkRadius = 10;
    [SerializeField] Color deadTileColor;

    [Header("Objects")]
    [SerializeField] GameObject mushroomPrefab;
    [SerializeField] float mushroomChance = 10;
    [SerializeField] List<WorldObjectData> mushroomObjetcs = new();
    [SerializeField] GameObject decorationPrefab;
    [SerializeField] float decorationChance = 30;
    [SerializeField] List<WorldObjectData> decorationObjects = new();
    [SerializeField] GameObject oxygenPlanetPrefab;
    [SerializeField] WorldObjectData oxygenPlantData;
    [SerializeField] float oxygenPlantChance = 3;
    [SerializeField] GameObject caveDecorationPrefab;
    [SerializeField] float caveDecorationChance = 10;
    [SerializeField] List<WorldObjectData> caveDecorationObjects = new();
    [SerializeField] WorldObjectData enemySpawnerObj;
    [SerializeField] GameObject enemySpawnerPrefab;
    [SerializeField] float enemySpawnerChance = 3;
    
    [Header("Star Setup")]
    [SerializeField] GameObject starPrefab;
    //[SerializeField] int starAmount;
    [SerializeField] float starChance = 60;
    [SerializeField] Vector2 starSizeRange;
    [SerializeField] Gradient starColorGradient;
    [Header("Dust Star Setup")]
    [SerializeField] GameObject dustPrefab;
    [SerializeField] int dustAmount;
    [SerializeField] Vector2 dustSizeRange;
    [SerializeField] float dustAlpha;
    [SerializeField] Gradient dustGradient;
    [SerializeField] GameObject bigStarPrefab;
    [SerializeField] float bigStarChance;

    bool isNew = true;
    bool hasMushroom = false;
    bool isOneEmpty;
    int seed;
    int oreangle=0;
    public bool canDisable = false;
    EnemySpawner spawner;
    SaveChunk saveChunk;
    int saveX;
    int saveY;

    List<GameObject> decorations = new();
    List<GameObject> bigDecorations = new();
    List<GameObject> caveDecorations = new();
    List<GameObject> dusts = new();
    List<GameObject> oxygenPlants = new();
    GameObject bigStar;

    public SaveChunk SaveData()
    {
        SaveChunk chunkData = new();
        chunkData.x = saveX;
        chunkData.y = saveY;
        foreach (GameObject decoObj in decorations)
        {
            if (decoObj == null) continue;
            chunkData.decorations.Add(new SaveDecoration
            {
                position = decoObj.transform.position,
                worldDataName = decoObj.GetComponent<InfoData>().data.visualName
            });
        }
        foreach (GameObject decoObj in bigDecorations)
        {
            if (decoObj == null) continue;
            chunkData.bigDecorations.Add(new SaveBigDecoration
            {
                position = decoObj.transform.position,
                worldDataName = decoObj.GetComponent<InfoData>().data.visualName
            });
        }
        foreach (GameObject opObj in oxygenPlants)
        {
            if (opObj == null) continue;
            chunkData.oxygenPlants.Add(new SaveOxygenPlant
            {
                position = opObj.transform.position,
            });
        }
        foreach (GameObject decoObj in caveDecorations)
        {
            if (decoObj == null) continue;
            chunkData.caveDecorations.Add(new SaveCaveDecoration
            {
                position = decoObj.transform.position,
                worldDataName = decoObj.GetComponent<InfoData>().data.visualName
            });
        }
        foreach (GameObject dustObj in dusts)
        {
            if (dustObj == null) continue;
            chunkData.dusts.Add(new SaveDust
            {
                localPosition = dustObj.transform.localPosition,
                color = dustObj.GetComponent<SpriteRenderer>().color,
                scale = dustObj.transform.localScale.x
            });
        }
        if (bigStar)
        {
            chunkData.bigStar = new SaveBigStar
            {
                localPosition = bigStar.transform.localPosition,
                index = bigStar.GetComponent<BigStar>().index,
                name = bigStar.GetComponent<BigStar>().starName,
                scale = bigStar.transform.localScale.x,
                alive = true
            };
        }
        if (spawner)
        {
            chunkData.spawner = new SaveSpawner
            {
                position = spawner.transform.position,
                health = spawner.GetComponent<EnemySpawner>().Health,
                alive = true
            };
        }
        return chunkData;
    }

    private void Awake()
    {
        saveX = (int)transform.position.x;
        saveY = (int)transform.position.y;
        seed = Constants.i.seed;
        saveChunk = SaveManager.i.GetChunkLoadedData(saveX, saveY);
        if (saveChunk != null)
            isNew = false;
        StartCoroutine(GenerateChunk());
    }

    public void DISABLE()
    {
        if (spawner != null)
        {
            spawner.CHUNKDISABLED();
        }
    }

    public void ENABLE()
    {
        if (spawner != null)
        {
            spawner.CHUNKENABLED();
        }
    }

    IEnumerator GenerateChunk()
    {
        for (int x = -chunkRadius; x <= chunkRadius; x++)
        {
            yield return new WaitForSeconds(0.00085f);
            // X LOOP
            int realX = (int)transform.position.x + x;
            float height = GetNoiseValue(realX + seed, 0, planetData.surfaceNoise) * planetData.heightMultiplier;
            bool done = false;
            for (int y = -chunkRadius; y <= chunkRadius; y++)
            {
                // Y LOOP
                WorldObjectData tileSelected = null;
                bool canGererate = false;
                bool canGenerateCaves = true;
                int realY = (int)transform.position.y + y;
                // TOP BLOCK
                if (realY == (int)height)
                {
                    tileSelected = planetData.topSprite;
                    canGererate = true;
                    canGenerateCaves = false;
                    if (!done)
                    {
                        done = true;
                        PlaceObject(realX, realY);
                    }
                }
                else if (realY < height)
                {
                    // BOTTOM BLOCK
                    if (realY < height - planetData.middleLayerHeight)
                    {
                        tileSelected = planetData.bottomSprite;
                        canGererate = true;
                        if (realY > height - planetData.middleLayerHeight - planetData.caveOffset)
                        {
                            canGenerateCaves = false;
                        }
                        // MIDDLE BLOCK
                    }
                    else
                    {
                        tileSelected = planetData.middleSprite;
                        canGererate = true;
                        canGenerateCaves = false;
                    }
                }
                // GENERATION
                if (canGererate)
                {
                    //float noiseValue = Mathf.PerlinNoise((realX+seed)*planetData.caveNoise.startFrequency,(realY+seed)*planetData.caveNoise.startFrequency);
                    float noiseValue = GetNoiseValue(realX + seed, realY, planetData.caveNoise);
                    if (noiseValue > planetData.isBlockValue || !canGenerateCaves)
                    {
                        bool rotate = false;
                        bool isOre = false;
                        float mineTime = 1.5f;
                        if (tileSelected == planetData.bottomSprite)
                        {
                            isOre = true;
                            foreach (OreSpawnSettings oresettings in planetData.oresSettings)
                            {
                                if (realY > height - oresettings.minHeight && realY < height - oresettings.maxHeight)
                                {
                                    float oreNoise = GetNoiseValue(realX+seed,realY,oresettings.noise);
                                    if (oreNoise > planetData.isOreValue)
                                    {
                                        tileSelected = oresettings.tile;
                                        rotate = true;
                                        mineTime = oresettings.mineTime;
                                        oreangle += 90;
                                        if (oreangle >= 360)
                                        {
                                            oreangle = 0;
                                        }
                                    }
                                }
                            }
                        }
                        PlaceTile(realX, realY, tileSelected, true,rotate,oreangle,isOre,mineTime);
                    }
                    else
                    {
                        PlaceTile(realX, realY, tileSelected, false,false,0,false,0);
                        PlaceCaveDecoration(realX, realY);
                    }
                }
                else
                {
                    isOneEmpty = true;
                    PlaceStar(realX, realY);
                }
                //yield return new WaitForSeconds(0.0001f);
            }
        }
        EndGeneration();
        canDisable = true;
    }

    void EndGeneration()
    {
        if (isOneEmpty)
        {
            PlaceDust();
            PlaceBigStar();
        }
        if (!isNew)
            LoadData();
    }

    void LoadData()
    {
        SaveChunk data = saveChunk;
        if (data==null) return;

        if (data.spawner.alive)
            CreateEnemySpawner(data.spawner.position, data.spawner.health);

        if (data.bigStar.alive)
            CreateBigStar(data.bigStar.localPosition, data.bigStar.index, data.bigStar.name, data.bigStar.scale);

        foreach (SaveOxygenPlant oxygenPlant in data.oxygenPlants)
            CreateOxygenPlant(oxygenPlant.position);

        foreach (SaveDecoration decoration in data.decorations)
            CreateDecoration(decoration.position, SaveManager.i.GetWorldDataFromName(decoration.worldDataName));

        foreach (SaveCaveDecoration decoration in data.caveDecorations)
            CreateCaveDecoration(decoration.position, SaveManager.i.GetWorldDataFromName(decoration.worldDataName));

        foreach (SaveBigDecoration decoration in data.bigDecorations)
            CreateBigDecoration(decoration.position, SaveManager.i.GetWorldDataFromName(decoration.worldDataName));

        foreach (SaveDust dust in data.dusts)
            CreateDust(dust.localPosition, dust.scale, dust.color);
    }

    void PlaceCaveDecoration(int realX, int realY){
        if (!isNew) return;

        if (Random.Range(0,100)<caveDecorationChance)
        {
            WorldObjectData selected = caveDecorationObjects[Random.Range(0, caveDecorationObjects.Count)];
            CreateCaveDecoration(new Vector3(realX + 0.5f, realY + 0.5f, 0), selected);
        }
        else if (Random.Range(0f, 100f) < enemySpawnerChance/80)
        {
            CreateEnemySpawner(new Vector3(realX + 0.5f, realY + 2, 0), -1);
        }
    }

    void PlaceObject(int realX, int realY){
        if (!isNew) return;

        if (!hasMushroom)
        {
            if (Random.Range(0, 100) < mushroomChance)
            {
                WorldObjectData selected = mushroomObjetcs[Random.Range(0, mushroomObjetcs.Count)];
                CreateBigDecoration(new Vector3(realX + 0.5f, realY + 2, 0), selected);
            }
            else if (Random.Range(0,100)<oxygenPlantChance)
            {
                CreateOxygenPlant(new Vector3(realX + 0.5f, realY + 2, 0));
            } else if (Random.Range(0f, 100f) < enemySpawnerChance)
            {
                CreateEnemySpawner(new Vector3(realX + 0.5f, realY + 2, 0), -1);
            }
        }
        else if (Random.Range(0, 100) < decorationChance)
        {
            WorldObjectData selected = decorationObjects[Random.Range(0, decorationObjects.Count)];
            CreateDecoration(new Vector3(realX + 0.5f, realY + 1.5f, 0), selected);
        }
    }

    void CreateOxygenPlant(Vector3 position)
    {
        GameObject m = Instantiate(oxygenPlanetPrefab, transform);
        m.transform.position = position;
        if (Random.Range(0f, 1f) < 0.5f)
        {
            m.GetComponent<SpriteRenderer>().flipX = true;
        }
        hasMushroom = true;
        m.GetComponent<InfoData>().Set(oxygenPlantData);
        oxygenPlants.Add(m);
    }

    void CreateEnemySpawner(Vector3 position, float health)
    {
        GameObject s = Instantiate(enemySpawnerPrefab, transform);
        s.transform.position = position;
        if (Random.Range(0f, 1f) < 0.5f)
        {
            s.GetComponent<SpriteRenderer>().flipX = true;
        }
        hasMushroom = true;
        s.GetComponent<InfoData>().Set(enemySpawnerObj);
        spawner = s.GetComponent<EnemySpawner>();
        if ((int)health != -1)
            spawner.SetHealth(health);
        spawner.FinishInit();
    }

    void CreateDecoration(Vector3 position, WorldObjectData objData)
    {
        GameObject d = Instantiate(decorationPrefab, transform);
        d.transform.position = position;
        d.GetComponent<SpriteRenderer>().sprite = objData.texture;
        d.GetComponent<InfoData>().Set(objData);
        WindObject windobj = d.AddComponent<WindObject>();
        windobj.rotationLimit = 8;
        decorations.Add(d);
    }

    void CreateCaveDecoration(Vector3 position, WorldObjectData objData)
    {
        GameObject d = Instantiate(caveDecorationPrefab, transform);
        d.transform.position = position;
        
        d.GetComponent<SpriteRenderer>().sprite = objData.texture;
        d.GetComponent<SpriteRenderer>().color = deadTileColor;
        if (objData.hasLight)
        {
            var light = d.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light.pointLightOuterRadius = 5;
            light.intensity = 0.5f;
        }
        d.GetComponent<InfoData>().Set(objData);
        caveDecorations.Add(d);
    }

    void CreateBigDecoration(Vector3 position, WorldObjectData objData)
    {
        GameObject m = Instantiate(mushroomPrefab, transform);
        m.transform.position = position;
        
        m.GetComponent<SpriteRenderer>().sprite = objData.texture;
        var light = m.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        light.color = objData.lightColor;

        if (Random.Range(0f, 1f) < 0.5f)
        {
            m.GetComponent<SpriteRenderer>().flipX = true;
        }
        hasMushroom = true;
        m.GetComponent<InfoData>().Set(objData);
        WindObject windobj = m.AddComponent<WindObject>();
        windobj.rotationLimit = 5;
        bigDecorations.Add(m);
    }

    void PlaceTile(int realX, int realY, WorldObjectData tileSelected, bool isAlive,bool rotate,int angle,bool isOre,float mineTime)
    {
        GameObject newTile = Instantiate(tilePrefab, transform);
        newTile.transform.position = new Vector3(realX + 0.5f, realY + 0.5f, 0);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSelected.texture;
        newTile.GetComponent<InfoData>().Set(tileSelected);
        if (!isAlive || SaveManager.i.WasTileMined(newTile))
        {
            newTile.GetComponent<SpriteRenderer>().color = deadTileColor;
            newTile.GetComponent<BoxCollider2D>().enabled = false;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        if (rotate)
        {
            newTile.transform.Rotate(0, 0, angle);
        }
        if (isOre)
        {
            Ore ore = newTile.AddComponent<Ore>();
            ore.dropItem = tileSelected.onDropItem;
            ore.amount = (int)(1000 * (Vector3.Distance(newTile.transform.position, Vector3.zero) / 100));
            ore.mineTime = mineTime;
        }
    }

    void PlaceDust()
    {
        if (!isNew) return;

        for (int i = 0; i < dustAmount; i++)
        {
            Vector3 position = new(Random.Range(-chunkRadius, chunkRadius), Random.Range(-chunkRadius, chunkRadius), 0);
            float size = Random.Range(dustSizeRange.x, dustSizeRange.y);
            Color color = dustGradient.Evaluate(Random.Range(0f, 1f));
            CreateDust(position, size, color);
        }
    }

    void CreateDust(Vector3 localPosition, float size, Color color)
    {
        GameObject newDust = Instantiate(dustPrefab, transform);
        newDust.transform.localPosition = localPosition;
        newDust.transform.localScale = new(size, size, 1);
        color.a = dustAlpha;
        newDust.GetComponent<SpriteRenderer>().color = color;
        dusts.Add(newDust);
    }

    void PlaceBigStar(){
        if (!isNew) return;

        if (transform.position.y >= 20){
            if (Random.Range(0f,100f)<starChance){
                Vector3 position = new(Random.Range(-chunkRadius, chunkRadius), Random.Range(-chunkRadius, chunkRadius), 0);
                GameObject newS = Instantiate(bigStarPrefab, transform);
                newS.transform.localPosition = position;
                bigStar = newS;
                newS.GetComponent<BigStar>().AutoSetup();
            }
        }
    }

    void CreateBigStar(Vector3 localPosition, int index, string starName, float scale)
    {
        GameObject newS = Instantiate(bigStarPrefab, transform);
        newS.transform.localPosition = localPosition;
        bigStar = newS;
        newS.GetComponent<BigStar>().ManualSetup(index, starName, scale);
    }

    void PlaceStar(int realX, int realY)
    {
        if (Random.Range(0, 100) < starChance)
        {
            Color color = starColorGradient.Evaluate(Random.Range(0, 1f));
            float size = Random.Range(starSizeRange.x, starSizeRange.y);
            CreateStar(realX, realY, color, size);
        }
    }

    void CreateStar(int realX, int realY, Color color, float size)
    {
        Vector3 position = new(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        
        GameObject newStar = Instantiate(starPrefab, transform);
        newStar.transform.position = new Vector3(realX, realY, 0) + position;
        newStar.transform.localScale = new Vector3(size, size, 1);
        newStar.GetComponent<SpriteRenderer>().color = color;
    }

    float GetNoiseValue(int x, int y, NoiseSettings settings)
    {
        float amplitude = 1;
        float frequency = settings.startFrequency;
        float noiseSum = 0;
        float amplitudeSum = 0;
        for (int i = 0; i < settings.octaves; i++)
        {
            noiseSum += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            amplitudeSum += amplitude;
            amplitude *= settings.persistence;
            frequency *= settings.frequencyMultiplier;
        }
        return noiseSum / amplitudeSum;
    }
}
