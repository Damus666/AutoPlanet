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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QUIT(){
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
