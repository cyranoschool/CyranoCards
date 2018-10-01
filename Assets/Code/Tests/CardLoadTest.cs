using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLoadTest : MonoBehaviour {

    public string cardPath;
    public bool SaveOnStart;
    public CardData data;

	// Use this for initialization
	void Start () {
        

        string path = SerializationManager.CreatePath("Cards/" + cardPath, SerializationManager.SavePathType.Persistent);

        //Test Save
        if (SaveOnStart)
        {
            SerializationManager.SaveJsonObject(path, data, true);
        }
        else
        {
            //Test Load
            data = SerializationManager.LoadJsonObject<CardData>(path);
        }
       

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
