using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayButtonSound : MonoBehaviour
{
    AudioSource clickSound;
    bool canPlaySound;

    private void Awake()
    {
        Button selfButton = GetComponent<Button>();
        if (!selfButton) return;
        GameObject soundManagerObj = GameObject.Find("Sounds");
        if (!soundManagerObj) return;
        SoundManager soundManager = soundManagerObj.GetComponent<SoundManager>();
        if (!soundManager) return;
        clickSound = soundManager.clickSound;
        selfButton.onClick.AddListener(new UnityAction(ONCLICK));
        canPlaySound = true;
    }

    public void ONCLICK()
    {
        if (!canPlaySound) return;
        clickSound.Play();
    }
}
