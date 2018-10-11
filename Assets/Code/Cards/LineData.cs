using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineData : CardData{

    //Currently references child cards explicitly by CardData 
    public List<string> CardsUID = new List<string>();

    //Note: this does not guarantee that the cards are loaded
    public List<CardData> GetCards()
    {
        throw new NotImplementedException();
        //The way lines get cards is slightly different, because base cards don't matter as much as the preferred line
        //return CardsUID.ConvertAll<CardData>(s => (CardData)CardManager.GetCardUID(s));
    }
}
