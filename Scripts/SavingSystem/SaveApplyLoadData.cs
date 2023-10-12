using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveApplyLoadData : MonoBehaviour
{
    [SerializeField] float waitTime = 3.1f;
    bool startedApplying;

    private void Update()
    {
        if (startedApplying) return;
        if (!SaveManager.i.isNewWorld)
        {
            Invoke(nameof(FinalApply), waitTime);
            startedApplying = true;
        }
        
    }

    void FinalApply()
    {
        SaveManager.i.ApplyLoadData();
        enabled = false;
    }
}
