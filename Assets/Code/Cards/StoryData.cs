using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryData : CardData{

    //Currently references child sections explicitly by SectionData 
    public List<string> SectionsUID = new List<string>();
}
