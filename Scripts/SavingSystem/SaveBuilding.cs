using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveCheckpoint
{
    public bool isPut;
    public string ID;
}

[System.Serializable]
public class SaveBuilding
{
    public Vector3 position;
    public int itemID;
    public bool needElectricityToInteract;
    public float health;
    public List<SaveCheckpoint> checkpoints = new();
    public List<SaveSlot> storages = new();
    public string strVar = string.Empty;
    public int intVar;
}
