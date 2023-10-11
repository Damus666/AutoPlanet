using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [SerializeField] AllItems allItems;
    [SerializeField] Player player;
    [SerializeField] Inventory inventory;
    [SerializeField] UnlockManager unlockManager;
    [SerializeField] ChunksGenerator chunksGenerator;
    [SerializeField] Constants constants;
    [SerializeField] BuildingManager buildingManager;
    [SerializeField] ToolInteract toolInteract;
    [SerializeField] GameObject loadingOverlay;
    [SerializeField] string jsonExtension = "json";
    [SerializeField] float loadTime = 4.0f;

    bool started;
    public SaveData saveData;
    public string worldName;
    public string fileName;
    public bool isNewWorld;
    public int seed;

    private void Awake()
    {
        fileName = Application.persistentDataPath + "/" + worldName + "." + jsonExtension;
        if (!isNewWorld)
            Load();
        started = true;
        loadingOverlay.SetActive(true);
        Invoke(nameof(DisableLoadingOverlay), loadTime);
    }

    void DisableLoadingOverlay()
    {
        loadingOverlay.SetActive(false);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        saveData = new SaveData();
        PutSaveData();

        string jsonData = JsonUtility.ToJson(saveData, false);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(fileName);
        bf.Serialize(fileStream, jsonData);
        fileStream.Close();

        print("DEBUG - SAVED");
    }

    public void PutSaveData()
    {
        saveData.seed = constants.seed;
        player.SaveData(saveData.savePlayer);
        inventory.SaveData(saveData.savePlayer);
        unlockManager.SaveData(saveData.savePlayer);
        toolInteract.SaveData(saveData);
        foreach (GameObject chunkObj in chunksGenerator.GetChunks())
        {
            if (chunkObj.TryGetComponent(out Chunk chunk)) 
            { 
                SaveChunk chunkData = chunk.SaveData();
                saveData.saveChunks.Add(chunkData);
            }
        }
        foreach (GameObject buildingObj in GameObject.FindGameObjectsWithTag("building"))
        {
            if (buildingObj.TryGetComponent(out Building building))
            {
                SaveBuilding buildingData = building.SaveData();
                saveData.saveBuildings.Add(buildingData);
            }
        }
        foreach (GameObject dropObj in GameObject.FindGameObjectsWithTag("drop"))
        {
            if (dropObj.TryGetComponent(out Drop drop))
            {
                SaveDrop dropData = drop.SaveData();
                saveData.saveDrops.Add(dropData);
            }
        }
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (started) return;
        if (!File.Exists(fileName)) return;

        saveData = new SaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(fileName, FileMode.Open);
        JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), saveData);
        file.Close();

        EarlyApplyLoadData();
        print("DEBUG - LOADED (+early applied)");
    }

    public void EarlyApplyLoadData()
    {
        seed = saveData.seed;
        player.LoadData(saveData.savePlayer);
    }

    public void ApplyLoadData()
    {
        inventory.LoadData(saveData.savePlayer, saveData.saveDrops);
        unlockManager.LoadData(saveData.savePlayer);
        buildingManager.LoadData(saveData.saveBuildings);

        print("DEBUG - APPLIED LOAD DATA");
    }

    public SaveChunk GetChunkLoadedData(int x, int y)
    {
        foreach (SaveChunk chunk in saveData.saveChunks)
        {
            if (chunk.x == x && chunk.y == y) return chunk;
        }
        return null;
    }

    public Item GetItemFromID(int ID)
    {
        foreach (Item item in allItems.items)
        {
            if (item.ID == ID) return item;
        }
        return null;
    }

    public WorldObjectData GetWorldDataFromName(string name)
    {
        foreach (WorldObjectData data in allItems.worldObjectDatas)
        {
            if (data.visualName == name) return data;
        }
        return null;
    }

    public bool WasTileMined(GameObject tile)
    {
        foreach (SaveIntPos pos in saveData.saveMinedBlocks)
        {
            if (pos.x == (int)tile.transform.position.x && pos.y == (int)tile.transform.position.y)
            {
                toolInteract.RegisterMinedTile(tile);
                return true;
            }
        }
        return false;
    }
}
