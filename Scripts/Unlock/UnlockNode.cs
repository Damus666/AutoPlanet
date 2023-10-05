using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Core/Create UnlockNode", fileName = "NewUnlockNode", order = 0)]
public class UnlockNode : ScriptableObject
{
    public string nodeName;
    public List<Item> itemsOutput = new();
    public bool isRoot = false;
    public List<UnlockNode> nodeRequirements = new();
    public List<UnlockNode> nodeChildren = new();
    public float unlockTime;
}
