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

    /// <summary>
    /// If dictionary already contains words matching this definition then refer to those instead
    /// </summary>
    public override void CheckDefinitionRepair(Dictionary<string, CardData> tempCards)
    {
        List<string> newUID = new List<string>();

        for (int i = 0; i < LinesUID.Count; i++)
        {
            string refID = LinesUID[i];
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
        LinesUID = newUID;
    }

    public override void AddCardReferences(List<CardData> cards)
    {
        base.AddCardReferences(cards);
        cards.ForEach(x => LinesUID.Add(x.UID));
    }

}
