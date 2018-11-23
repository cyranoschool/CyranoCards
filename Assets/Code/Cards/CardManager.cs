using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardManager
{

    public readonly static CardManager Instance = new CardManager();

    public enum Direction { From, To }

    Dictionary<string, List<CardData>> cardsFromAll = new Dictionary<string, List<CardData>>();
    Dictionary<string, List<CardData>> cardsToAll = new Dictionary<string, List<CardData>>();
    Dictionary<string, CardData> cardsUID = new Dictionary<string, CardData>();
    //This has to be a Dictionary as you can't get values back out of hashset (which is important due to the custom equality check)
    //Temporarily removed (should be a list of matching cards)
    //Dictionary<CardData, CardData> matchingDefinitions;

    int totalCards = 0;
    int totalFromCollisions = 0;
    int totalToCollisions = 0;

    public CardManager()
    {
        //matchingDefinitions = new Dictionary<CardData, CardData>(new CardData.DefinitionComparer());
    }

    public static List<CardData> LoadFolder(string folderName, bool placeInDictionary = true, SerializationManager.SavePathType PathType = SerializationManager.SavePathType.Streaming, SearchOption searchOption = SearchOption.AllDirectories)
    {
        return Instance.loadFolder(folderName, placeInDictionary, PathType, searchOption);
    }

    List<CardData> loadFolder(string folderName, bool placeInDictionary = true, SerializationManager.SavePathType PathType = SerializationManager.SavePathType.Streaming, SearchOption searchOption = SearchOption.AllDirectories)
    {
        string folderPath = SerializationManager.CreatePath(folderName, PathType);
        folderPath = folderPath.TrimEnd(new char[] { '\\', '/' }) + "/";
        var info = new DirectoryInfo(folderPath);
        if (!info.Exists)
        {
            Debug.LogWarning($"Folder {folderPath} doesn't exist!");
            return new List<CardData>();
        }
        var fileInfo = info.GetFiles("*.json", searchOption);
        List<CardData> cardsLoaded = new List<CardData>();
        foreach (FileInfo file in fileInfo)
        {
            cardsLoaded.Add(LoadCard(file.FullName, placeInDictionary));
        }
        return cardsLoaded;
    }

    CardData LoadCard(string path, bool placeInDictionary = true)
    {
        string cardText = SerializationManager.LoadJsonText(path);
        CardData card = JsonUtility.FromJson<CardData>(cardText);
        //Card needs to be loaded from json as the derived type before casting so data isn't lost
        CardData fullCard = (CardData)JsonUtility.FromJson(cardText, Type.GetType(card.CardType));
        if (placeInDictionary)
        {
            PlaceInDictionaries(fullCard);
        }
        return fullCard;
    }

    public static void PlaceInDictionaries(CardData card)
    {
        //If card already exists unload old card and replace with this one
        
        /* Matching definitions commented out to allow for cards to have multiple concurrent definitions
        if (Instance.matchingDefinitions.ContainsKey(card))
        {
            Instance.UnloadCard(Instance.matchingDefinitions[card]);
        }
        */
        //Create both from and too collections
        Instance.SetupCard(card, Direction.From);
        Instance.SetupCard(card, Direction.To);
        Instance.cardsUID.Add(card.UID, card);

        //Instance.matchingDefinitions.Add(card, card);
        
    }


    void SetupCard(CardData card, Direction direction)
    {
        Dictionary<string, List<CardData>> allCards;
        //Currently key is the text and not the name of the file
        //This means there can be duplicates of words, from or to
        string key;
        if (direction == Direction.From)
        {
            allCards = cardsFromAll;
            key = card.From;

        }
        else
        {
            allCards = cardsToAll;
            key = card.To;
        }
        //All keys are lowercase to prevent confusion
        key = key.ToLower();
        List<CardData> list;
        if (!allCards.TryGetValue(key, out list))
        {
            list = new List<CardData>();
            allCards.Add(key, list);
        }
        else
        {
            //Another card also has this key
            if (direction == Direction.From)
            {
                totalFromCollisions++;
            }
            else
            {
                totalToCollisions++;
            }
        }
        list.Add(card);
        totalCards++;
    }

    void UnloadCard(CardData cardData)
    {
        string from = cardData.From.ToLower();
        string to = cardData.To.ToLower();
        List<CardData> potentialCards;
        if(cardsFromAll.TryGetValue(from, out potentialCards))
        {
            potentialCards.Remove(cardData);
        }
        if(cardsToAll.TryGetValue(to, out potentialCards))
        {
            potentialCards.Remove(cardData);
        }
        cardsUID.Remove(cardData.UID);

        totalCards--;
    }

    public static void UnloadAll(bool unloadTextures = true)
    {
        Instance.cardsFromAll.Clear();
        Instance.cardsToAll.Clear();
        Instance.cardsUID.Clear();
        //Instance.matchingDefinitions.Clear();
        if(unloadTextures)
        {
            Resources.UnloadUnusedAssets();
        }
    }

    public static List<CardData> GetCards(string text, Direction direction)
    {
        switch (direction)
        {
            case Direction.From:
                return GetCardsFrom(text);
            case Direction.To:
                return GetCardsTo(text);
        }
        //Can never happen
        //This is for compiler warnings
        return new List<CardData>(0);
    }
    public static CardData GetCardUID(string UID)
    {
        CardData card = null;
        Instance.cardsUID.TryGetValue(UID, out card);
        return card;
    }

    public static IEnumerable<CardData> GetAllCards()
    {
        return Instance.cardsUID.Values;
    }

    public static List<CardData> GetCardsFrom(string text)
    {
        List<CardData> value;
        return Instance.cardsFromAll.TryGetValue(text.ToLower(), out value) ? value : new List<CardData>(0);
    }
    public static List<CardData> GetCardsTo(string text)
    {
        List<CardData> value;
        return Instance.cardsToAll.TryGetValue(text.ToLower(), out value) ? value : new List<CardData>(0);
    }

    public static bool SaveCard(CardData cardData, string path, bool prettyPrint = true, bool useDefaultName = true)
    {
        string name = "";
        if(useDefaultName)
        {
            name = "/" + cardData.UID + ".json";
        }
        return SerializationManager.SaveJsonObject(path + name, cardData, prettyPrint);
    }

    /// <summary>
    /// Return a duplicated card object
    /// </summary>
    /// <param name="cardData">Card Data to duplicate</param>
    /// <param name="placeInDictionary">Put this card into the current dictionary after creation</param>
    /// <returns></returns>
    public static CardData GetDuplicateCard(CardData cardData, bool placeInDictionary = false)
    {
        CardData clone = new CardData();
        clone.CloneFrom(cardData);

        if(placeInDictionary)
        {
            PlaceInDictionaries(clone);
        }
        return clone;
    }

    /*
    public static bool ContainsMatchingDefinition(CardData card)
    {
        return Instance.matchingDefinitions.ContainsKey(card);
    }

    public static CardData GetMatchingDefinition(CardData card)
    {
        return Instance.matchingDefinitions[card];
    }
    */
}