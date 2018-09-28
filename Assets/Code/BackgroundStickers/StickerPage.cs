using UnityEngine;
using System.Collections.Generic;
using System;

public class StickerPage : MonoBehaviour
{

	StickerPageData pageData = new StickerPageData();
    List<GameObject> stickers = new List<GameObject>();

    const string SAVE_PATH = "Stickers/";
	
	public void LoadPage(string stickerPageName)
	{
		//Currently JsonLoading
		string location = SerializationManager.CreatePath(SAVE_PATH + stickerPageName);
		StickerPageData data = SerializationManager.LoadJsonObject<StickerPageData>(location);
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
            //Set sprite name here
        }
        pageData.stickers = stickerData;
	}
	
	public void SavePage(string fileName)
	{
		UpdatePageData();
		//Currently using JsonSaving
		string location = SerializationManager.CreatePath(SAVE_PATH + fileName);
		SerializationManager.SaveJsonObject(location, pageData);
	}

   
    public GameObject CreateSticker(string name)
    {
        throw new NotImplementedException(); 
    }
    
}

