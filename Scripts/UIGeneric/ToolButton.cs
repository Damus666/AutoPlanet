using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    public int index;
    [SerializeField] Tools tools;

    public void ONCLICK(){
        tools.SelectTool(index);
    }
}
