using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryData : CardData{

    //Currently references child sections explicitly by SectionData 
    public List<string> SectionsUID = new List<string>();

    //Note: this does not guarantee that the cards are loaded
    public List<SectionData> GetSectionCards()
    {
        return SectionsUID.ConvertAll<SectionData>(s => (SectionData)CardManager.GetCardUID(s));
    }
    public override List<CardData> GetChildCards()
    {
        return SectionsUID.ConvertAll<CardData>(s => (CardData)CardManager.GetCardUID(s));
    }
    public override int ChildCardCount()
    {
        return SectionsUID.Count;
    }

    public override void AddCardReferences(List<CardData> cards)
    {
        base.AddCardReferences(cards);
        cards.ForEach(x => SectionsUID.Add(x.UID));
    }
}
