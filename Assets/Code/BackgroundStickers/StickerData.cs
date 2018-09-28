using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class StickerData
{
	public Vector3 position;
	public string sprite = "";
}

[Serializable]
public class StickerPageData
{
	public List<StickerData> stickers = new List<StickerData>();
}

