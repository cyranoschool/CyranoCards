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

    /// <summary>
    /// If dictionary already contains words matching this definition then refer to those instead
    /// Should be static
    /// </summary>
    public override void CheckDefinitionRepair(Dictionary<string, CardData> tempCards)
    {
        List<string> newUID = new List<string>();

        for (int i = 0; i < SectionsUID.Count; i++)
        {
            string refID = SectionsUID[i];
            CardData refCard = tempCards[refID];
            if (CardManager.ContainsMatchingDefinition(refCard))
            {
                newUID.Add(CardManager.GetMatchingDefinition(refCard).UID);
            }
            else
            {
                newUID.Add(refID);
            }
        }
        SectionsUID = newUID;
    }

    public override void AddCardReferences(List<CardData> cards)
    {
        base.AddCardReferences(cards);
        cards.ForEach(x => SectionsUID.Add(x.UID));
    }
}
