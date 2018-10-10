using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CardData
{
    //Unused and up to change
    //public string Icon = "";
    //public bool Favorited = false;
    //public string creator = "";
    //public int creatorID = 0;
    //public string dateTime = "";


    public string From = "";
    public string To = "";

    public string BrokenUpFrom = "";
    public string BrokenUpTo = "";

    public string UID = Guid.NewGuid().ToString(); //Unique ID

}
