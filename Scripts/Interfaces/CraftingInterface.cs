using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategoryHolder
{
    public List<SubCategoryHolder> subCategories;
}

[System.Serializable]
public class SubCategoryHolder
{
    public List<Item> items;
}

public class CraftingInterface : Interface
{
    [Header("------------------------------------------------------")]
    [SerializeField] List<GameObject> categories;
    [SerializeField] List<CategoryHolder> items;
    [SerializeField] GameObject subCategoryPrefab;
    [SerializeField] GameObject craftingSlotPrefab;
    [SerializeField] CraftingQueue queue;
    List<CraftingSlot> slots = new();

    private void Awake()
    {
        int i = 0;
        foreach (CategoryHolder category in items)
        {
            foreach (SubCategoryHolder subcategory in category.subCategories)
            {
                GameObject newSubCategory = Instantiate(subCategoryPrefab, categories[i].transform);
                foreach (Item item in subcategory.items)
                {
                    GameObject newSlot = Instantiate(craftingSlotPrefab, newSubCategory.transform);
                    newSlot.GetComponent<CraftingSlot>().Setup(item,inventory, queue);
                    slots.Add(newSlot.GetComponent<CraftingSlot>());
                }
            }
            i++;
        }
        gameObject.SetActive(false);
    }
    /*
    private void Update()
    {
        foreach (CraftingSlot slot in slots)
        {
            slot.CheckCanCraft();
        }
    }*/

    public void SELECTCATEGORY(int buttonIndex)
    {
        int i = 0;
        foreach (GameObject c in categories)
        {
            if (i == buttonIndex)
            {
                c.SetActive(true);
            } else
            {
                c.SetActive(false);
            }
            i++;
        }
    }

    protected override void OnClose()
    {
        foreach (var hover in slots)
        {
            hover.POINTEREXIT();
        }
    }
}
