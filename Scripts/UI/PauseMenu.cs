using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject contentHolder;
    [SerializeField] List<HoverOutlineEffect> hoverOutlineEffects = new();

    public void RESUME()
    {
        Close();
    }

    public void SAVE()
    {
        SaveManager.i.Save();
    }

    public void SAVEBACK()
    {
        SaveManager.i.Save();
        Close();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void SAVEQUIT()
    {
        SaveManager.i.Save();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void QUIT()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Open()
    {
        contentHolder.SetActive(true);
        StateManager.i.paused = true;
        Time.timeScale = 0;
    }

    public void Close()
    {
        contentHolder.SetActive(false);
        StateManager.i.paused = false;
        Time.timeScale = 1;
        foreach (HoverOutlineEffect effect in hoverOutlineEffects)
        {
            effect.POINTEREXIT();
        }
    }

    private void Update()
    {
        if (!StateManager.i.inventoryOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!contentHolder.activeSelf) Open();
            else Close();
        }
    }
}
