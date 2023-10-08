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
    [Header("Base")]public string itemName;
    public int ID;
    [TextArea] public string description;
    public Sprite texture;

    [Header("Building")] public bool isBuilding;
    public BuildingData buildingData;

    [Header("Crafting")] public CraftType carftType = CraftType.HandsAndCrafter;
    public float craftTime;
    public Item smeltedVersion;
    public List<Requirement> requirements;
    public int craftAmount;

    [Header("Details")] public int stackSize;
    public bool startsUnlocked;
    public int experience;
    public float processTime;
    public bool halfScaleDrop = false;
    public float maxHealth;

    [Header("Special")] public int specialID=-1;
    public int specialLevel = 0;
}
