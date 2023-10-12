using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    public int index;

    public void ONCLICK(){
        Tools.i.SelectTool(index);
    }
}
