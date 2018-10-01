using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CardData
{
    public List<string> Children = new List<string>();
    public List<string> Icons = new List<string>();
    public string From = "";
    public string To = "";
}
