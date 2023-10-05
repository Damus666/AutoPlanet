using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public bool inventoryOpen;
    public Interface currentInterface;

    public void SetInterface(Interface i)
    {
        currentInterface = i;
    }
}
