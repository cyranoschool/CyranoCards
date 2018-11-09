using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineData : CardData{

    //Currently references child cards explicitly by CardData 
    public List<string> CardsUID = new List<string>();

    //Note: this does not guarantee that the cards are loaded
    public List<CardData> GetWordCards()
    {
        return CardsUID.ConvertAll<CardData>(s => (CardData)CardManager.GetCardUID(s));
    }
    public override List<CardData> GetChildCards()
    {
        return CardsUID.ConvertAll<CardData>(s => (CardData)CardManager.GetCardUID(s));
    }
    public override int ChildCardCount()
    {
        return CardsUID.Count;
    }

    /// <summary>
    /// If dictionary already contains words matching this definition then refer to those instead
    /// </summary>
    public override void CheckDefinitionRepair(Dictionary<string, CardData> tempCards)
    {
        List<string> newUID = new List<string>();

        for (int i = 0; i < CardsUID.Count; i++)
        {
            string refID = CardsUID[i];
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
        CardsUID = newUID;
    }

    public override void AddCardReferences(List<CardData> cards)
    {
        base.AddCardReferences(cards);
        cards.ForEach(x => CardsUID.Add(x.UID));
    }

}
