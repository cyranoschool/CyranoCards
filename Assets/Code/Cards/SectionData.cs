using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionData : CardData
{

    //Currently references child lines explicitly by LineData 
    public List<string> LinesUID = new List<string>();

    //Note: this does not guarantee that the cards are loaded
    public List<LineData> GetLineCards()
    {
        return LinesUID.ConvertAll<LineData>(s => (LineData)CardManager.GetCardUID(s));
    }

    public override void AddCardReferences(List<CardData> cards)
    {
        base.AddCardReferences(cards);
        cards.ForEach(x => LinesUID.Add(x.UID));
    }

}
