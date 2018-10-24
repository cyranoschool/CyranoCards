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
    public List<string> FavoritedCardsUID;
    HashSet<string> favoritedCardsSet = new HashSet<string>();

    public List<string> FriendedUsersUID;
    HashSet<string> friendedUsersSet = new HashSet<string>();

    public List<ParentPins> ParentPinnedCards;
    Dictionary<string, ParentPins> parentPinnedCardsSetUID = new Dictionary<string, ParentPins>();
    
    [Serializable]
    public class ParentPins
    {
        public string CardUID = "";
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
            if(index < PinnedCardsUID.Count)
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

    /// <summary>
    /// Remove CardUID from list of cards User has favorited
    /// </summary>
    /// <param name="CardUID"></param>
    /// <returns>true if the element is removed from the HashSet<T> object; false if the element didn't exist.</returns>
    public bool UnfavoriteCardUID(string CardUID)
    {
        return favoritedCardsSet.Remove(CardUID);
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

    public Dictionary<string, ParentPins> GetParentPins()
    {
        return parentPinnedCardsSetUID;
    }
    

    public void OnAfterDeserialize()
    {
        favoritedCardsSet = new HashSet<string>(FavoritedCardsUID);
        FavoritedCardsUID = null;

        friendedUsersSet = new HashSet<string>(FriendedUsersUID);
        FriendedUsersUID = null;

        parentPinnedCardsSetUID = new Dictionary<string, ParentPins>();
        ParentPinnedCards.ForEach(x => parentPinnedCardsSetUID.Add(x.CardUID, x));
        ParentPinnedCards = null;
        
    }

    public void OnBeforeSerialize()
    {
        FavoritedCardsUID = favoritedCardsSet.ToList();

        FriendedUsersUID = friendedUsersSet.ToList();

        ParentPinnedCards = parentPinnedCardsSetUID.Values.ToList();
    }
}
