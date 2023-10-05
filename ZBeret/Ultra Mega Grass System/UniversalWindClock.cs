using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalWindClock : MonoBehaviour
{
    public float windSpeed;

    [Header("Ignore this from the editor"), Range(0f, 1f)]
    public float windTurnAmount;
    float invisTimer;

    void Update(){
        invisTimer += windSpeed * Time.deltaTime;
        windTurnAmount = Mathf.PingPong(invisTimer, 1);
    }
}
