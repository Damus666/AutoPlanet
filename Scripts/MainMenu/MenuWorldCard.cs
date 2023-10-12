using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class MenuWorldCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameTxt;

    MenuPlay menuPlay;
    string worldName;

    public void Setup(MenuPlay menuPlay, string worldName)
    {
        this.menuPlay = menuPlay;
        this.worldName = worldName;
        nameTxt.text = worldName;
    }

    public void DELETECLICK()
    {
        menuPlay.DeleteWorld(worldName);
    }

    public void PLAYCLICK()
    {
        menuPlay.PlayWorld(worldName, 0);
    }
}
