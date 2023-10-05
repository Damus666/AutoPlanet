using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExpedition : MonoBehaviour
{
    [SerializeField] float expeditionCooldown;
    [SerializeField] float distanceForExpedition;
    [SerializeField] float maxExpeditionTime;
    float lastExpedition;
    Constants constants;
    EnemySpawner spawner;
    Transform pT;
    bool isExpedition;

    public void SetSpawner(EnemySpawner s,Transform p)
    {
        spawner = s;
        pT = p;
        constants = GameObject.Find("GameManager").GetComponent<Constants>();
    }

    private void Update()
    {
        if (!constants.enemyExpeditionActive)
        {
            if (Time.time-lastExpedition > expeditionCooldown && Time.time-constants.lastExpedition > expeditionCooldown)
            {
                float dist = Vector3.Distance(transform.position, pT.transform.position);
                if (dist <= distanceForExpedition)
                {
                    spawner.SendExpedition();
                    constants.enemyExpeditionActive = true;
                    lastExpedition = Time.time;
                    isExpedition = true;
                    constants.lastExpedition = Time.time;
                }
            }
        } 
        if (isExpedition)
        {
            if (Time.time-lastExpedition > maxExpeditionTime)
            {
                constants.enemyExpeditionActive = false;
            }
        }
    }
}
