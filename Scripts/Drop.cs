using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public Item item;
    SpriteRenderer spriteRenderer;
    Inventory inventory;

    public void Setup(Item item)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.item = item;
        spriteRenderer.sprite = item.texture;
        InfoData data = GetComponent<InfoData>();
        data.visualName = item.itemName + " (Drop)";
        data.description = item.description;
        data.texture = item.texture;
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 3 || collision.gameObject.layer == 10) && !collision.isTrigger)
        {
            int remaining = inventory.AddItem(item, 1);
            if (remaining == 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 3 || collision.gameObject.layer == 10) && !collision.isTrigger)
        {
            int remaining = inventory.AddItem(item, 1);
            if (remaining == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
