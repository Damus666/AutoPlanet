using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigStar : MonoBehaviour
{
    [SerializeField] Vector2 sizeRange = new(1,1.5f);
    [SerializeField] List<Sprite> sprites = new();
    [SerializeField] List<Color> dustColors = new();
    [SerializeField] SpriteRenderer dustRenderer;
    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string numbers = "1234567890";
    public string starName;
    public int index;

    public void AutoSetup()
    {
        index = Random.Range(0, sprites.Count);
        GetComponent<SpriteRenderer>().sprite = sprites[index];
        InfoData info = GetComponent<InfoData>();
        info.texture = sprites[index];
        info.description = "Star ID: " + GenID();
        starName = info.description;
        Color c = dustColors[index];
        c.a = 0.5f;
        dustRenderer.color = c;
        float size = Random.Range(sizeRange.x, sizeRange.y);
        transform.localScale = new Vector3(size, size, 1);
    }

    public void ManualSetup(int index, string starName, float size)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[index];
        InfoData info = GetComponent<InfoData>();
        info.texture = sprites[index];
        info.description = starName;
        Color c = dustColors[index];
        c.a = 0.5f;
        dustRenderer.color = c;
        transform.localScale = new Vector3(size, size, 1);
        this.starName = starName;
        this.index = index;
    }

    public string GenID()
    {
        string final = "";
        for (int _ = 0; _ < 3; _++)
        {
            final += letters[Random.Range(0, letters.Length)];
        }
        for (int _ = 0; _ < 3; _++)
        {
            final += numbers[Random.Range(0, numbers.Length)];
        }
        return final;
    }
}
