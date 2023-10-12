using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuPlay : MonoBehaviour
{
    [SerializeField] MenuActions menuActions;
    [SerializeField] string jsonExtension;
    [SerializeField] GameObject worldCardPrefab;

    [SerializeField] TMP_InputField worldNameInput;
    [SerializeField] Transform cardHolder;
    [SerializeField] GameObject subTitleTxt;

    List<GameObject> worldCards = new();
    bool tempAddAllItemsStart;

    private void Awake()
    {
        PlayerPrefs.SetInt("TEMPaddAllItemsStart", 0);
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);

        foreach (GameObject card in worldCards)
            Destroy(card);
        
        DirectoryInfo dir = new(Application.persistentDataPath);
        FileInfo[] infos = dir.GetFiles("*." + jsonExtension);
        foreach (FileInfo fileInfo in infos)
        {
            string worldName = fileInfo.Name.Split(".")[0];
            GameObject card = Instantiate(worldCardPrefab, cardHolder);
            card.GetComponent<MenuWorldCard>().Setup(this, worldName);
            worldCards.Add(card);
        }

        subTitleTxt.SetActive(worldCards.Count <= 0 ? false : true);
    }

    public void DeleteWorld(string name)
    {
        string fileName = Application.persistentDataPath + "/" + name + "." + jsonExtension;
        if (File.Exists(fileName))
            File.Delete(fileName);
        OnOpen();
    }

    public void PlayWorld(string name, int isNew)
    {
        PlayerPrefs.SetString("worldName", name);
        PlayerPrefs.SetInt("isNew", isNew);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NEWWORLDCLICK()
    {
        if (worldNameInput.text.Trim() == string.Empty) return;
        PlayWorld(worldNameInput.text, 1);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        menuActions.MenuClosed();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void TEMPTOGGLE()
    {
        tempAddAllItemsStart = !tempAddAllItemsStart;
        PlayerPrefs.SetInt("TEMPaddAllItemsStart", tempAddAllItemsStart ? 1 : 0);
    }
}
