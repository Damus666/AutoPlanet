using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Data/Create Object Data", fileName = "NewObjectData", order = 2)]
public class WorldObjectData : ScriptableObject
{
    public string visualName;
    [TextArea] public string description;
    public Sprite texture;
    public Color lightColor;
    public bool hasLight;
    public Item onDropItem;
}
