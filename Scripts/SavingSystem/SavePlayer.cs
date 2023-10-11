using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavePlayer
{
    public Vector3 position;
    public List<SaveSlot> inventorySlots = new();
    public SaveSlot bodySlot;
    public SaveSlot petSlot;
    public List<int> unlockedIDs = new();
    public float health;
    public int unlockXP;
    public int unlockPoints;
    public int nextLevelXP;
    public List<string> unlockedNodes = new();
    public bool isResearching;
    public string nextUnlockNodeName = string.Empty;
}
