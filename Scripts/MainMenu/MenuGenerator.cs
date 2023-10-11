using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGenerator : MonoBehaviour
{
    [Header("Amounts")]
    [SerializeField] float genRadius;
    [SerializeField] int bigStarAmount;
    [SerializeField] int smallStarAmount;
    [SerializeField] int dustAmount;

    [Header("Prefabs")]
    [SerializeField] GameObject bigStarPrefab;
    [SerializeField] GameObject smallStarPrefab;
    [SerializeField] GameObject dustPrefab;
    [SerializeField] Transform contentHolderT;

    [Header("Settings")]
    [SerializeField] Vector2 smallStarSizeRange;
    [SerializeField] Gradient smallStarColorGradient;
    [SerializeField] Vector2 dustSizeRange;
    [SerializeField] Gradient dustGradient;
    [SerializeField] float dustAlpha;

    [Header("Buttons")]
    [SerializeField] List<SpriteRenderer> buttonDusts = new();

    private void Awake()
    {
        for (int i = 0; i < smallStarAmount; i++)
            CreateSmallStar();
        for (int i = 0; i < dustAmount; i++)
            CreateDust();
        for (int i = 0; i < bigStarAmount; i++)
            CreateBigStar();

        foreach (SpriteRenderer dustR in buttonDusts)
        {
            Color color = dustGradient.Evaluate(Random.Range(0f, 1f));
            color.a = dustAlpha;
            dustR.color = color;
        }
        
    }

    void CreateDust()
    {
        GameObject newDust = Instantiate(dustPrefab, contentHolderT);
        newDust.transform.localPosition = new(Random.Range(-genRadius, genRadius), Random.Range(-genRadius, genRadius), 0);

        float size = Random.Range(dustSizeRange.x, dustSizeRange.y);
        newDust.transform.localScale = new(size, size, 1);

        Color color = dustGradient.Evaluate(Random.Range(0f, 1f));
        color.a = dustAlpha;
        newDust.GetComponent<SpriteRenderer>().color = color;
    }

    void CreateBigStar()
    {
        GameObject newS = Instantiate(bigStarPrefab, contentHolderT);
        newS.transform.localPosition = new(Random.Range(-genRadius, genRadius), Random.Range(-genRadius, genRadius), 0); ;
        newS.GetComponent<BigStar>().AutoSetup();
    }

    void CreateSmallStar()
    {
        GameObject newStar = Instantiate(smallStarPrefab, contentHolderT);
        newStar.transform.localPosition = new(Random.Range(-genRadius, genRadius), Random.Range(-genRadius, genRadius), 0);

        float size = Random.Range(smallStarSizeRange.x, smallStarSizeRange.y);
        newStar.transform.localScale = new Vector3(size, size, 1);

        Color color = smallStarColorGradient.Evaluate(Random.Range(0, 1f));
        newStar.GetComponent<SpriteRenderer>().color = color;
    }
}
