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
    Dictionary<string, CardData> cardsGUID = new Dictionary<string, CardData>();

    int totalCards = 0;
    int totalFromCollisions = 0;
    int totalToCollisions = 0;

    public static void LoadFolder(string folderName, SearchOption searchOption = SearchOption.AllDirectories)
    {
        Instance.loadFolder(folderName, searchOption);
    }

    void loadFolder(string folderName, SearchOption searchOption = SearchOption.AllDirectories)
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "SaveData/" + folderName + "/");
        var info = new DirectoryInfo(folderPath);
        if (!info.Exists)
        {
            Debug.LogError($"Folder {folderPath} doesn't exist!");
            return;
        }
        var fileInfo = info.GetFiles("*.json", searchOption);
        foreach (FileInfo file in fileInfo)
        {

            LoadCard(file.FullName, file.Name);
        }
    }

    void LoadCard(string path, string fileName)
    {
        CardData card = SerializationManager.LoadJsonObject<CardData>(path);
        //Create both from and too collections
        SetupCard(card, fileName, Direction.From);
        SetupCard(card, fileName, Direction.To);
        cardsGUID.Add(card.UID, card);
    }


    void SetupCard(CardData card, string fileName, Direction direction)
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
        cardsGUID.Remove(cardData.UID);
    }

    public static void UnloadAll()
    {
        Instance.cardsFromAll.Clear();
        Instance.cardsToAll.Clear();
        Instance.cardsGUID.Clear();
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

    public static bool SaveCard(CardData cardData, string path)
    {
        return SerializationManager.SaveJsonObject(path, cardData);
    }

    /// <summary>
    /// Return a duplicated card object
    /// </summary>
    /// <param name="cardData">Card Data to duplicate</param>
    /// <param name="placeInDictionary">Put this card into the current dictionary after creation</param>
    /// <returns></returns>
    public static CardData DuplicateCard(CardData cardData, bool placeInDictionary = true)
    {
        //Save into temporary folder and reload it
        string path = SerializationManager.CreatePath("ClonedCard.json", SerializationManager.SavePathType.TempCache);
        SerializationManager.SaveJsonObject(path, cardData);
        CardData clone = SerializationManager.LoadJsonObject<CardData>(path);

        if(placeInDictionary)
        {
            Instance.SetupCard(clone,"", Direction.From);
            Instance.SetupCard(clone, "", Direction.To);

        }
        return clone;
    }

}