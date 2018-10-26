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

    public List<string> FriendedUsersUID;
    
    HashSet<string> friendedUsersSet = new HashSet<string>();

    public List<LocalUserCardData> LocalUserCards;
    
    Dictionary<string, LocalUserCardData> localUserCards = new Dictionary<string, LocalUserCardData>();

    public LocalUserCardData GetOrCreateLocalCardData(string UID, bool placeInDictionary = true)
    {
        LocalUserCardData data = null;
        if (!localUserCards.TryGetValue(UID, out data))
        {
            data = new LocalUserCardData { CardUID = UID };
            if(placeInDictionary)
            {
                localUserCards.Add(UID, data);
            }
        }
        return data;
    }


    public bool IsCardUIDFavorited(string cardUID)
    {
        LocalUserCardData data = null;
        if (localUserCards.TryGetValue(cardUID, out data))
        {
            return data.Favorited;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Add CardUID to list of cards User has favorited
    /// </summary>
    /// <param name="CardUID"></param>
    /// <returns>true if the element is added to the HashSet<T> object; false if the element is already present.</returns>
    public bool FavoriteCardUID(string cardUID)
    {
        LocalUserCardData data = GetOrCreateLocalCardData(cardUID);
        return data.Favorited = true;
    }

    /// <summary>
    /// Remove CardUID from list of cards User has favorited
    /// </summary>
    /// <param name="CardUID"></param>
    /// <returns>true if the element is removed from the HashSet<T> object; false if the element didn't exist.</returns>
    public bool UnfavoriteCardUID(string cardUID)
    {
        LocalUserCardData data = GetOrCreateLocalCardData(cardUID);
        return data.Favorited = false;
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

    /// <summary>
    /// Add UserUID to list of friends User is friends with (or followed)
    /// </summary>
    /// <param name="UserUID"></param>
    /// <returns>true if the element is removed from the HashSet<T> object; false if the element didn't exist.</returns>
    public bool UnfriendUserUID(string UserUID)
    {
        return friendedUsersSet.Remove(UserUID);
    }

    public ParentPins GetParentPin(string UID)
    {
        return GetOrCreateLocalCardData(UID).ParentPinnedCards;
    }
    
    /// <summary>
    /// This should only be edited in the case that A localcarddata that was returned wasn't placed in dictionary
    /// Waiting to place in dictionary can be useful if only reading data, where the returned data can be used as default values
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, LocalUserCardData> GetLocalUserCards()
    {
        return localUserCards;
    }



    public void OnAfterDeserialize()
    {
        friendedUsersSet = new HashSet<string>(FriendedUsersUID);
        FriendedUsersUID = null;

        localUserCards = new Dictionary<string, LocalUserCardData>();
        LocalUserCards.ForEach(x => localUserCards.Add(x.CardUID, x));
        LocalUserCards = null;
        
    }

    public void OnBeforeSerialize()
    {
        FriendedUsersUID = friendedUsersSet.ToList();

        LocalUserCards = localUserCards.Values.ToList();
    }
}
