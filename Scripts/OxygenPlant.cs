using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenPlant : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    public bool isActive = true;
    [SerializeField] float refilAmount = 30f;
    [SerializeField] float waitTime=60;
    EnergySource energySource;

    void Awake(){
        particles.Play();
    }

    public void ResetActive(){
        isActive = true;
        particles.Play();
    }

    public void SetEnergySource(EnergySource source)
    {
        energySource = source;
    }

    private void OnDestroy()
    {
        if (energySource != null)
        {
            energySource.PlantDestroyed();
        }
    }
}
