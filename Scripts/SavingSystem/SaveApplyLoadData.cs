using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveApplyLoadData : MonoBehaviour
{
    [SerializeField] SaveManager saveManager;
    [SerializeField] float waitTime = 3.1f;
    bool startedApplying;

    private void Update()
    {
        if (startedApplying) return;
        if (!saveManager.isNewWorld)
        {
            Invoke(nameof(FinalApply), waitTime);
            startedApplying = true;
        }
        
    }

    void FinalApply()
    {
        saveManager.ApplyLoadData();
        enabled = false;
    }
}
