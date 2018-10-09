using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionData : CardData
{

    //Currently references child lines explicitly by LineData 
    public List<LineData> Lines = new List<LineData>();
}
