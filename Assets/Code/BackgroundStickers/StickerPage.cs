using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;

public class StickerPage : MonoBehaviour
{
	StickerPageData pageData;

    const string SAVE_PATH = "Stickers/";
	
	public void SetupPage(string stickerPageName)
	{
		//Currently JsonLoading
		string location = SerializationManager.CreatePath(SAVE_PATH + stickerPageName);
		StickerPageData data = SerializationManager.LoadJsonObject<StickerPageData>(location);
		SetupPage(data);
	}
	
	public void SetupPage(StickerPageData data)
	{
		pageData = data;
		ReloadPage();
	}
	
	public void ReloadPage()
	{
		
	}
	
	void UpdatePageData()
	{
		
	}
	
	public void SavePage(string fileName)
	{
		UpdatePageData();
		//Currently using JsonSaving
		string location = SerializationManager.CreatePath(SAVE_PATH + fileName);
		SerializationManager.SaveJsonObject(location, pageData);
	}
	
}

