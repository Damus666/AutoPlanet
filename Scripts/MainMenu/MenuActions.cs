using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActions : MonoBehaviour
{
    [SerializeField] MenuPlay menuPlay;

    public AudioSource clickSound;
    public bool menuOpen;

    public void ONPLAY()
    {
        if (menuOpen) return;
        menuPlay.OnOpen();
        MenuOpened();
    }

    public void ONSETTINGS()
    {
        if (menuOpen) return;
        //MenuOpened();
    }

    public void MenuClosed()
    {
        menuOpen = false;
    }

    void MenuOpened()
    {
        menuOpen = true;
        clickSound.Play();
    }

    public void ONQUIT()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
