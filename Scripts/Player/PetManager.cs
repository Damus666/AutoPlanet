using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetManager : MonoBehaviour
{
    Pet currentPet;
    [SerializeField] Constants constants;
    [SerializeField] GameObject petPrefab;

    public void OnPetSlotChange(Slot petSlot)
    {
        if (petSlot.isEmpty)
        {
            Destroy(currentPet.gameObject);
            currentPet = null;
        }
        else
        {
            if (currentPet != null)
            {
                if (petSlot.item.specialID != currentPet.specialID)
                {
                    Destroy(currentPet.gameObject);
                    currentPet = null;
                    CreateNewPet(petSlot.item.specialID);
                }
            } else
            {
                CreateNewPet(petSlot.item.specialID);
            }
        }
    }

    void CreateNewPet(int ID)
    {
        switch (ID)
        {
            case 3:
                var newpet = Instantiate(petPrefab);
                currentPet = newpet.AddComponent<LightPet>();
                currentPet.Setup(constants, transform.parent, ID);
                break;
            case 4:
                var newpet2 = Instantiate(petPrefab);
                currentPet = newpet2.AddComponent<LaserPet>();
                currentPet.Setup(constants, transform.parent, ID);
                break;
            case 5:
                var newpet3 = Instantiate(petPrefab);
                currentPet = newpet3.AddComponent<CollectorPet>();
                currentPet.Setup(constants, transform.parent, ID);
                break;
        }
    }
}
