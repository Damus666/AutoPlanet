using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class DeathButtons : MonoBehaviour
{
    [SerializeField] float alphaSpeed = 1;
    [SerializeField] CanvasGroup group;

    public void RESTART(){
        SaveManager.i.Save();
        PlayerPrefs.SetString("worldName", SaveManager.i.worldName);
        PlayerPrefs.SetInt("isNew", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BACK()
    {
        SaveManager.i.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void QUIT(){
        SaveManager.i.Save();
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }
    
    private void Update() {
        if (group.alpha < 1){
            group.alpha += Time.deltaTime * alphaSpeed;
        }
    }
}
