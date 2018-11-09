using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardExtensions
{

    /// <summary>
    /// Sort List of CardData by their scores
    /// </summary>
    /// <param name="cardDataList">List to sort from</param>
    /// <param name="ascending">Sort by score ascending or descending (default = ascending)</param>
    /// <returns></returns>
    public static List<CardData> SortByScore(this List<CardData> cardDataList, bool ascending = true)
    {
        UserData user = UserManager.Instance.GetCurrentUser();

        var userCards = user.GetLocalUserCards();

        //Use an empty LocalUserCardData to be used in the case that value is missing
        //This prevents needless allocations of LocalUserCardData's from user.GetOrCreateLocalCardData(UID, false)
        LocalUserCardData localDefault = new LocalUserCardData();
        IOrderedEnumerable<CardData> ordered;
        if (ascending)
        {
            ordered = cardDataList.OrderBy(x => userCards.GetValueOrDefault(x.UID, localDefault).Progress);
        }
        else
        {
            ordered = cardDataList.OrderByDescending(x => userCards.GetValueOrDefault(x.UID, localDefault).Progress);

        }
        
        return ordered.ToList();
    }

}