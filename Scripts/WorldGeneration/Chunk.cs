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
    bool hasMushroom = false;
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

    bool isOneEmpty;
    int seed;
    int oreangle=0;
    public bool canDisable = false;
    EnemySpawner spawner;

    private void Awake()
    {
        seed = GameObject.Find("GameManager").GetComponent<Constants>().seed;
        StartCoroutine(GenerateChunk());
        //GenerateChunk();
        
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
            yield return new WaitForSeconds(0.001f);
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
                        PLaceObject(realX, realY);
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
    }

    void PlaceCaveDecoration(int realX, int realY){
        if (Random.Range(0,100)<caveDecorationChance){
            GameObject d = Instantiate(caveDecorationPrefab,transform);
            d.transform.position = new Vector3(realX+0.5f, realY+0.5f,0);
            WorldObjectData selected = caveDecorationObjects[Random.Range(0, caveDecorationObjects.Count)];
            d.GetComponent<SpriteRenderer>().sprite = selected.texture;
            d.GetComponent<SpriteRenderer>().color = deadTileColor;
            if (selected.hasLight){
                var light = d.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
                light.pointLightOuterRadius = 5;
                light.intensity = 0.5f;
            }
            d.GetComponent<InfoData>().Set(selected);
        }
        else if (Random.Range(0f, 100f) < enemySpawnerChance/80)
        {
            GameObject s = Instantiate(enemySpawnerPrefab, transform);
            s.transform.position = new Vector3(realX + 0.5f, realY + 2, 0);
            if (Random.Range(0f, 1f) < 0.5f)
            {
                s.GetComponent<SpriteRenderer>().flipX = true;
            }
            hasMushroom = true;
            s.GetComponent<InfoData>().Set(enemySpawnerObj);
            spawner = s.GetComponent<EnemySpawner>();
            spawner.FinishInit();
        }
    }

    void PLaceObject(int realX, int realY){
        if (!hasMushroom)
        {
            if (Random.Range(0, 100) < mushroomChance){
            GameObject m = Instantiate(mushroomPrefab, transform);
            m.transform.position = new Vector3(realX + 0.5f, realY + 2, 0);
            int index = Random.Range(0, mushroomObjetcs.Count);
            WorldObjectData s = mushroomObjetcs[index];
            m.GetComponent<SpriteRenderer>().sprite = s.texture;
            var light = m.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
            light.color = s.lightColor;
            
            if (Random.Range(0f, 1f) < 0.5f)
            {
                m.GetComponent<SpriteRenderer>().flipX = true;
            }
            hasMushroom = true;
                m.GetComponent<InfoData>().Set(s);
                WindObject windobj = m.AddComponent<WindObject>();
                windobj.rotationLimit = 5;
            }
            else if (Random.Range(0,100)<oxygenPlantChance){
                GameObject m = Instantiate(oxygenPlanetPrefab, transform);
                m.transform.position = new Vector3(realX + 0.5f, realY + 2, 0);
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    m.GetComponent<SpriteRenderer>().flipX = true;
                }
                hasMushroom = true;
                m.GetComponent<InfoData>().Set(oxygenPlantData);
            } else if (Random.Range(0f, 100f) < enemySpawnerChance)
            {
                GameObject s = Instantiate(enemySpawnerPrefab, transform);
                s.transform.position = new Vector3(realX + 0.5f, realY + 2, 0);
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    s.GetComponent<SpriteRenderer>().flipX = true;
                }
                hasMushroom = true;
                s.GetComponent<InfoData>().Set(enemySpawnerObj);
                spawner = s.GetComponent<EnemySpawner>();
                spawner.FinishInit();
            }
        }
        else if (Random.Range(0, 100) < decorationChance)
        {
            GameObject d = Instantiate(decorationPrefab, transform);
            d.transform.position = new Vector3(realX + 0.5f, realY + 1.5f, 0);
            WorldObjectData selected = decorationObjects[Random.Range(0, decorationObjects.Count)];
            d.GetComponent<SpriteRenderer>().sprite = selected.texture;
            d.GetComponent<InfoData>().Set(selected);
            WindObject windobj = d.AddComponent<WindObject>();
            windobj.rotationLimit = 8;
        }
    }

    void PlaceTile(int realX, int realY, WorldObjectData tileSelected, bool isAlive,bool rotate,int angle,bool isOre,float mineTime)
    {
        GameObject newTile = Instantiate(tilePrefab, transform);
        newTile.transform.position = new Vector3(realX + 0.5f, realY + 0.5f, 0);
        newTile.GetComponent<SpriteRenderer>().sprite = tileSelected.texture;
        newTile.GetComponent<InfoData>().Set(tileSelected);
        if (!isAlive)
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
        for (int i = 0; i < dustAmount; i++)
        {
            Vector3 position = new(Random.Range(-chunkRadius, chunkRadius), Random.Range(-chunkRadius, chunkRadius), 0);
            float size = Random.Range(dustSizeRange.x, dustSizeRange.y);
            GameObject newDust = Instantiate(dustPrefab, transform);
            newDust.transform.localPosition = position;
            newDust.transform.localScale = new(size, size, 1);
            Color color = dustGradient.Evaluate(Random.Range(0f, 1f));
            color.a = dustAlpha;
            newDust.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void PlaceBigStar(){
        if (transform.position.y >= 20){
        if (Random.Range(0f,100f)<starChance){
            Vector3 position = new(Random.Range(-chunkRadius, chunkRadius), Random.Range(-chunkRadius, chunkRadius), 0);
            GameObject newS = Instantiate(bigStarPrefab, transform);
            newS.transform.localPosition = position;
        }
        }
    }

    void PlaceStar(int realX, int realY)
    {
        if (Random.Range(0, 100) < starChance)
        {
            Vector3 position = new(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            float size = Random.Range(starSizeRange.x, starSizeRange.y);
            GameObject newStar = Instantiate(starPrefab, transform);
            newStar.transform.position = new Vector3(realX, realY, 0) + position;
            newStar.transform.localScale = new Vector3(size, size, 1);
            Color color = starColorGradient.Evaluate(Random.Range(0, 1f));
            newStar.GetComponent<SpriteRenderer>().color = color;
        }
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
