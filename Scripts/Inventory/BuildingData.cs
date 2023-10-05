using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData 
{
    public InterfaceType targetInterface = InterfaceType.Generic;
    public bool destroyVegetation = true;
    public int buildingScale = 1;
    public bool doGetData = true;
    public bool needElectricity = true;
    public bool canInteract = true;
}
