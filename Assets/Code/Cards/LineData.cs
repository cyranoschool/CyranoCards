using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineData : CardData{

    //Currently references child cards explicitly by CardData 
    public List<CardData> Cards = new List<CardData>();
}
