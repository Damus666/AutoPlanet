using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static StateManager i;
    public bool inventoryOpen;
    public bool paused;
    public Interface currentInterface;

    private void Awake()
    {
        i = this;
    }

    public void SetInterface(Interface i)
    {
        currentInterface = i;
    }
}
