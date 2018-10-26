using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Data stored for individual cards on the user side
/// For example a user may have favorited a card
/// That favorite isn't stored on the card as the card could come from anywhere, it's per user
/// </summary>
[Serializable]
public class LocalUserCardData {
    public string CardUID = "";
    public bool Favorited = false;
    public ParentPins ParentPinnedCards = null;

    public LocalUserCardData() { }

}

[Serializable]
public class ParentPins
{
    public List<string> PinnedCardsUID = new List<string>(0);

    public void SetCard(string UID, int index)
    {
        //Add empty indices if there are no pinned cards at those spots yet
        while (PinnedCardsUID.Count <= index)
        {
            PinnedCardsUID.Add("");
        }
        PinnedCardsUID[index] = UID;
    }

    public string GetUIDAtIndex(int index)
    {
        string UID;
        if (index < PinnedCardsUID.Count)
        {
            UID = PinnedCardsUID[index];
        }
        else
        {
            UID = "";
        }
        return UID;
    }
}