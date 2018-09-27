using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class StickerData
{
	public Vector3 location;
	public string sprite;
}

[Serializable]
public class StickerPageData
{
	public List<StickerData> stickers = new List<StickerData>();
	public string backgroundSprite;
}

