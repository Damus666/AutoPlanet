using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDecoration
{
    public Vector3 position;
    public string worldDataName;
}

[System.Serializable]
public class SaveBigDecoration
{
    public Vector3 position;
    public string worldDataName;
}

[System.Serializable]
public class SaveCaveDecoration
{
    public Vector3 position;
    public string worldDataName;
}

[System.Serializable]
public class SaveOxygenPlant
{
    public Vector3 position;
}

[System.Serializable]
public class SaveDust
{
    public Vector3 localPosition;
    public float scale;
    public Color color;
}

[System.Serializable]
public class SaveBigStar
{
    public Vector3 localPosition;
    public float scale;
    public int index;
    public string name;
    public bool alive;
}

[System.Serializable]
public class SaveSpawner
{
    public Vector3 position;
    public float health;
    public bool alive;
}