using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CraftType
{
    OnlySmelting,
    OnlyCrafter,
    HandsAndCrafter,
    OnlyHands,
    CannotCraft,
}

[CreateAssetMenu(menuName = "Core/Create Item", fileName = "NewItem", order = 0)]
public class Item : ScriptableObject
{
    public string itemName;
    public int ID;
    [TextArea] public string description;
    public Sprite texture;
    public int stackSize;
    public bool isBuilding;
    public BuildingData buildingData;
    public CraftType carftType = CraftType.HandsAndCrafter;
    public float craftTime;
    public Item smeltedVersion;
    public List<Requirement> requirements;
    public int craftAmount;
    public int specialID=-1;
    public int specialLevel = 0;
    public bool startsUnlocked;
    public int experience;
    public float processTime;
    public bool halfScaleDrop = false;
    public float maxHealth;
}
