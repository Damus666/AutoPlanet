using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActions : MonoBehaviour
{
    public AudioSource clickSound;

    public void ONPLAY()
    {

    }

    public void ONSETTINGS()
    {

    }

    public void ONQUIT()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
