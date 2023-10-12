using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllItems : MonoBehaviour
{
    public static AllItems i;
    public List<Item> items;
    public List<WorldObjectData> worldObjectDatas = new();

    private void Awake()
    {
        i = this;
    }
}
