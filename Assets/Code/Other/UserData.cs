using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class UserData : ISerializationCallbackReceiver
{
    public string Name = "Unnamed";
    //Currently using GUID for unique ID
    //In a networked system the user id should just be the ID of the last user + 1
    public string UID = Guid.NewGuid().ToString();

    //This is only used for serialization. To see if card is favorited use IsCardUIDFavorited
    public List<string> FavoritedCardsUID = new List<string>(0);
    HashSet<string> favoritedCardsSet;

    public List<string> FriendedUsersUID = new List<string>(0);
    HashSet<string> friendedUsersSet;


    public bool IsCardUIDFavorited(string CardUID)
    {
        return favoritedCardsSet.Contains(CardUID);
    }

    /// <summary>
    /// Add CardUID to list of cards User has favorited
    /// </summary>
    /// <param name="CardUID"></param>
    /// <returns>true if the element is added to the HashSet<T> object; false if the element is already present.</returns>
    public bool FavoriteCardUID(string CardUID)
    {
        return favoritedCardsSet.Add(CardUID);
    }


    public bool IsUserUIDFriended(string UserUID)
    {
        return friendedUsersSet.Contains(UserUID);
    }

    /// <summary>
    /// Add UserUID to list of friends User is friends with (or followed)
    /// </summary>
    /// <param name="UserUID"></param>
    /// <returns>true if the element is added to the HashSet<T> object; false if the element is already present.</returns>
    public bool FriendUserUID(string UserUID)
    {
        return friendedUsersSet.Add(UserUID);
    }

    public void OnAfterDeserialize()
    {
        favoritedCardsSet = new HashSet<string>(FavoritedCardsUID);
        FavoritedCardsUID = null;

        friendedUsersSet = new HashSet<string>(FriendedUsersUID);
        FriendedUsersUID = null;
    }

    public void OnBeforeSerialize()
    {
        FavoritedCardsUID = favoritedCardsSet.ToList();

        FriendedUsersUID = friendedUsersSet.ToList();
    }
}
