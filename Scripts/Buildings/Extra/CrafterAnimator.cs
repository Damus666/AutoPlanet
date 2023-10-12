using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrafterAnimator : MonoBehaviour
{
    [SerializeField] GameObject nozzle;
    [SerializeField] GameObject bar;
    [SerializeField] float startHeight;
    [SerializeField] float heightStep;
    [SerializeField] float itemWidth;
    float craftTime;
    float horizontalTime;
    float maxRight;
    float maxLeft;
    Vector3 barOffset;
    Vector3 nozzleOffset;
    float direction = 1;
    float startTime;
    float lastTime;

    private void Awake()
    {
        maxRight = itemWidth / 2;
        maxLeft = -itemWidth / 2;
    }

    public void StartAnimation(float craftTime)
    {
        this.craftTime = craftTime;
        horizontalTime = craftTime / 5;
        lastTime = Time.time;
        startTime = Time.time;
        nozzleOffset.x = maxLeft;
        nozzleOffset.y = startHeight;
        barOffset.y = nozzleOffset.y;
        Update();
    }

    private void Update()
    {
        float x = (Time.time - lastTime) / horizontalTime;
        float y = (Time.time - startTime) / craftTime;
        if (direction > 0)
        {
            nozzleOffset.x = Mathf.Lerp(maxLeft, maxRight, x);
            if (nozzleOffset.x >= maxRight)
            {
                direction *= -1;
                lastTime = Time.time;
            }
        } else
        {
            nozzleOffset.x = Mathf.Lerp(maxRight, maxLeft, x);
            if (nozzleOffset.x <= maxLeft)
            {
                direction *= -1;
                lastTime = Time.time;
            }
        }
        nozzleOffset.y = Mathf.Lerp(startHeight, startHeight + heightStep * 5, y);
        barOffset.y = nozzleOffset.y;

        bar.transform.localPosition = barOffset;
        nozzle.transform.localPosition = nozzleOffset;
    }
}
