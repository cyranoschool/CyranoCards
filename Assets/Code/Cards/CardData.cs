﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CardData
{
    //Values duplicated

    public string From = "";
    public string To = "";
    public string BrokenUpFrom = "";
    public string BrokenUpTo = "";
    //public string Icon = "";

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
        BrokenUpFrom = cardData.BrokenUpFrom;
        BrokenUpTo = cardData.BrokenUpTo;
        //Icon = cardData.Icon;
    }
}
