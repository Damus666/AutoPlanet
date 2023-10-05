using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindObject : MonoBehaviour
{
    public float rotationLimit;
    UniversalWindClock uwc;

    Vector3 leftRot;
    Vector3 rightRot;

    void Start(){
        uwc = GameObject.Find("GameManager").GetComponent<UniversalWindClock>();
        leftRot = new Vector3(0, 0, -rotationLimit);
        rightRot = new Vector3(0, 0, rotationLimit);
    }

    void Update(){
        this.transform.rotation = Quaternion.Lerp(Quaternion.Euler(leftRot), Quaternion.Euler(rightRot), uwc.windTurnAmount);
    }
}
