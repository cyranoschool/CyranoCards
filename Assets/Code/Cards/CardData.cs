using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CardData
{
    //Values duplicated

    public string From = "";
    public string To = "";
    public string PhoneticFrom = "";
    public string BrokenUpTo = "";
    public string Icon = "";

    //Values not duplicated

    public string UID = Guid.NewGuid().ToString(); //Unique ID
    //public bool Favorited = false;
    //public string creator = Get current creator here;
    //public int creatorID = only this field is required instead of creator field;
    public long DateTicks = DateTime.UtcNow.Ticks;

    //Values constant
    //public string CardType = "CardData"; //This is used to denote what the inherited type is for json
    public string CardType;

    public CardData()
    {
        CardType = this.GetType().FullName;
    }

    public virtual void Duplicate(CardData cardData)
    {
        //Some values for new card shouldn't be duplicated
        //e.g. favorited, creator, creation date, UID
        From = cardData.From;
        To = cardData.To;
        PhoneticFrom = cardData.PhoneticFrom;
        BrokenUpTo = cardData.BrokenUpTo;
        //Icon = cardData.Icon;
    }

    /// <summary>
    /// Finalize card creation (Take all language strings in CardData and trim and force lowercase)
    /// </summary>
    public virtual void DataFinalize()
    {
        From = From.ToLower();
        To = To.ToLower();
        PhoneticFrom = PhoneticFrom.ToLower();
        BrokenUpTo = BrokenUpTo.ToLower();
    }

    /// <summary>
    /// If children of card already exist in Dictionary, replace them with already occuring card
    /// </summary>
    public virtual void CheckDefinitionRepair(Dictionary<string, CardData> tempCards)
    {

    }

    /// <summary>
    /// Cards with the same From and Broken up value are considerd equal for the use of checking if card has already been made 
    /// </summary>
    public class DefinitionComparer : EqualityComparer<CardData>
    {
        public override bool Equals(CardData c1, CardData c2)
        {
            return c1.From.Equals(c2.From) && c1.BrokenUpTo.Equals(c2.BrokenUpTo) && c1.CardType.Equals(c2.CardType);
        }
        public override int GetHashCode(CardData c)
        {
            return c.From.GetHashCode() ^ c.To.GetHashCode();
        }
    }

    public virtual void AddCardReferences(List<CardData> cards)
    {

    }

    /// <summary>
    /// Breaks up card into its base words and makes cards for those words
    /// Intended for use in LineData,SectionData and StoryData
    /// </summary>
    /// <returns></returns>
    public List<CardData> GenerateWordCards()
    {
        List<CardData> wordCards = new List<CardData>();
        string[] wordsFrom = From.Split();
        //string[] wordsTo = To.Split();
        string[] wordsPhon = PhoneticFrom.Split();
        string[] wordsBrokenUp = BrokenUpTo.Split();

        int length = wordsFrom.Length;
        if(wordsPhon.Length != length || wordsBrokenUp.Length != length)
        {
            Debug.LogError($"Line not matching:\n{To}\n" +
                $"FromLength: {wordsFrom.Length}\n" +
                $"PhonLength: {wordsPhon.Length}\n" +
                $"BrokenUpLength: {wordsBrokenUp.Length}");
            return wordCards;
        }

        for (int i = 0; i < wordsFrom.Length; i++)
        {
            CardData card = new CardData();
            card.From = wordsFrom[i];
            card.To = wordsBrokenUp[i];
            card.PhoneticFrom = wordsPhon[i];
            card.BrokenUpTo = wordsBrokenUp[i];
            wordCards.Add(card);
        }
        return wordCards;
    }
}
