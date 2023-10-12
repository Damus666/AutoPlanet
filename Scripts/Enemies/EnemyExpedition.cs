using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExpedition : MonoBehaviour
{
    [SerializeField] float expeditionCooldown;
    [SerializeField] float distanceForExpedition;
    [SerializeField] float maxExpeditionTime;

    EnemySpawner spawner;
    Transform pT;

    float lastExpedition;
    bool isExpedition;

    public void SetSpawner(EnemySpawner s,Transform p)
    {
        spawner = s;
        pT = p;
    }

    private void Update()
    {
        if (!Constants.i.enemyExpeditionActive)
        {
            if (Time.time-lastExpedition > expeditionCooldown && Time.time- Constants.i.lastExpedition > expeditionCooldown)
            {
                float dist = Vector3.Distance(transform.position, pT.transform.position);
                if (dist <= distanceForExpedition)
                {
                    spawner.SendExpedition();
                    Constants.i.enemyExpeditionActive = true;
                    lastExpedition = Time.time;
                    isExpedition = true;
                    Constants.i.lastExpedition = Time.time;
                }
            }
        } 
        if (isExpedition)
        {
            if (Time.time-lastExpedition > maxExpeditionTime)
            {
                Constants.i.enemyExpeditionActive = false;
            }
        }
    }
}
