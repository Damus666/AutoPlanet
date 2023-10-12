using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRuntime : MonoBehaviour
{
    [SerializeField] SpriteRenderer image;
    [SerializeField] Color okColor;
    [SerializeField] Color badColor;

    [SerializeField] List<Transform> corners;
    [SerializeField] public List<Transform> blockChecks;

    [SerializeField] public GameObject energyRangeIndicator;
    [SerializeField] public GameObject healthBar;
    [SerializeField] LayerMask petLayer;
    [SerializeField] Collider2D thisCollider;

    public bool isValid;
    public bool isPlaced;

    public void Setup(Sprite image, bool isEnergy)
    {
        this.image.sprite = image;
        if (!isEnergy)
        {
            Destroy(energyRangeIndicator);
        }
    }

    public void OnPlace(bool destroyVegetation)
    {
        thisCollider.enabled = false;
        image.color = Color.white;
        if (destroyVegetation)
        {
            foreach (Transform corner in corners)
            {
                RaycastHit2D hit = Physics2D.Raycast(corner.position, Vector3.zero,1, ~petLayer);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == 7)
                    {
                        Inventory.i.SpawnDrop(hit.collider.GetComponent<InfoData>().data.onDropItem, hit.collider.transform.position);
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
        thisCollider.enabled = true;
        enabled = false;
        isValid = true;
        gameObject.layer = 6;
        if (energyRangeIndicator != null)
        {
            Destroy(energyRangeIndicator);
        }
        Invoke(nameof(SetPlaced), 0.5f);
    }

    void SetPlaced()
    {
        isPlaced = true;
    }

    private void Update()
    {
        thisCollider.enabled = false;
        bool atLeastOne = false;
        foreach (Transform corner in corners)
        {
            RaycastHit2D hit = Physics2D.Raycast(corner.position, Vector3.zero, 1, ~petLayer);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == 6 || hit.collider.gameObject.CompareTag("building") || hit.collider.gameObject.layer == 3)
                {
                    atLeastOne = true;   
                }
            }
        }
        bool hasNoBlock = false;
        foreach (Transform blockCheck in blockChecks)
        {
            RaycastHit2D hit2 = Physics2D.Raycast(blockCheck.position, Vector3.zero, 1, ~petLayer);
            if (hit2.collider != null)
            {
                if (hit2.collider.gameObject.layer != 6 || (hit2.collider.gameObject.CompareTag("building") && hit2.collider.GetComponent<Building>().referenceItem.buildingData.targetInterface != InterfaceType.Block))
                {
                    hasNoBlock = true;
                }
            } else
            {
                hasNoBlock = true;
            }
        }
        thisCollider.enabled = true;
        if (atLeastOne || hasNoBlock || Inventory.i.isMouseHovering)
        {
            image.color = badColor;
            isValid = false;
        } else
        {
            image.color = okColor;
            isValid = true;
        }
    }
}
