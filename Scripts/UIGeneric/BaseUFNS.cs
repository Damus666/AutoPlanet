using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUFNS : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool waitBeforeDisappearing;
    [SerializeField] float disappearSpeed;
    [SerializeField] protected float waitTime;
    protected UFNS ufns;
    protected bool canDisappear=false;

    public void ResetAlpha()
    {
        canvasGroup.alpha = 1;
    }

    protected void SetDisappear()
    {
        canDisappear = true;
    }

    public void BaseSetup(UFNS ufns)
    {
        this.ufns = ufns;
        Invoke(nameof(SetDisappear),waitTime);
    }

    protected virtual void AboutToDestroy()
    {

    }

    private void Update()
    {
        if (canDisappear || !waitBeforeDisappearing)
        {
            canvasGroup.alpha -= Time.deltaTime * disappearSpeed;
        }
        if (canvasGroup.alpha <= 0)
        {
            AboutToDestroy();
            Destroy(gameObject);
        }
    }
}
