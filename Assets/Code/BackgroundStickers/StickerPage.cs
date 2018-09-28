using UnityEngine;
using System.Collections.Generic;
using System;

public class StickerPage : MonoBehaviour
{
    public List<GameObject> StickerPrefabs;
    public bool BinaryFile = false; //Save and load file in binary

    Dictionary<string, GameObject> StickerKeys = new Dictionary<string, GameObject>();
	StickerPageData pageData = new StickerPageData();
    List<GameObject> stickers = new List<GameObject>();

    const string SAVE_PATH = "Stickers/";

    public void Awake()
    {
        RebuildStickerKeys();
    }

    public void Update()
    {
        
    }

    public void Start()
    {
        //TestSave();
        //LoadPage("test");
    }

    /// <summary>
    /// Takes list of StickerPrefabs and inputs them in a dictionary for loading
    /// </summary>
    public void RebuildStickerKeys()
    {
        StickerKeys.Clear();
        foreach(GameObject g in StickerPrefabs)
        {
            StickerKeys.Add(g.name, g);
        }
    }

    /// <summary>
    /// Takes objects in children and adds them to the sticker list and saves them
    /// </summary>
    public void TestSave()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            stickers.Add(transform.GetChild(i).gameObject);
        }
        SavePage("test", true);
    }

    public void LoadPage(string stickerPageName)
	{
		string path = SerializationManager.CreatePath(SAVE_PATH + stickerPageName);
        StickerPageData data;
        if (BinaryFile)
        {
            //BinaryFormmater implementation
            path += ".bytes";
            data = (StickerPageData)SerializationManager.LoadObject(path);
        }
        else
        {
            path += ".json";
            //Json implementation
            data = SerializationManager.LoadJsonObject<StickerPageData>(path);
        }
        LoadPage(data);
	}
	
	public void LoadPage(StickerPageData data)
	{
		pageData = data;
		ReloadPage();
	}

    public void ClearPage()
    {
        foreach (GameObject g in stickers)
        {
            Destroy(g);
        }
        stickers.Clear();
    }

	public void ReloadPage()
	{
        ClearPage();
        //Build page based on pagedata
        foreach (StickerData sD in pageData.stickers)
        {
            //Create sticker sprites in their proper locations
            GameObject g = CreateSticker(sD.sprite);
            g.transform.parent = transform;
            g.transform.position = sD.position;
            stickers.Add(g);
        }
    }
	
	void UpdatePageData()
	{
        List<StickerData> stickerData = new List<StickerData>();
		foreach(GameObject g in stickers)
        {
            StickerData sD = new StickerData();
            sD.position = g.transform.position;
            sD.sprite = g.name;
            stickerData.Add(sD);
        }
        pageData.stickers = stickerData;
	}
	
	public void SavePage(string fileName, bool prettyPrint = false)
	{
		UpdatePageData();

		string path = SerializationManager.CreatePath(SAVE_PATH + fileName + ".json");
        if(BinaryFile)
        {
            path += ".bytes";
            SerializationManager.SaveObject(path, pageData);
        }
        else
        {
            path += ".json";
            SerializationManager.SaveJsonObject(path, pageData, prettyPrint);
        }
        
    }

    /// <summary>
    /// Creates new instance of sticker
    /// </summary>
    /// <param name="name">Name of sticker in prefab list to spawn</param>
    /// <returns></returns>
    public GameObject CreateSticker(string name)
    {
        GameObject g = StickerKeys[name];
        if(g == null)
        {
            Debug.LogError("Sticker: {0} is not a prefab in the list!");
            //Could return a default sticker instead of null
            return null;
        }
        return GameObject.Instantiate(g);
    }
    
}

