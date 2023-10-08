using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RobocloneInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] TMP_InputField botIDInput;
    public Roboclone currentBot;

    public void BotOpen(Roboclone clone)
    {
        currentBot = clone;
        botIDInput.text = currentBot.botID;
    }

    public void BOTIDCHANGE(string text)
    {
        currentBot.botID = text;
        currentBot.CHECKPOINTCHANGE();
    }

    public override bool CanClose()
    {
        return !botIDInput.isFocused;
    }
}
