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

    int totalCards = 0;
    int totalFromCollisions = 0;
    int totalToCollisions = 0;

    public void LoadFolder(string folderName)
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "SaveData/" + folderName + "/");
        var info = new DirectoryInfo(folderPath);
        if (!info.Exists)
        {
            Debug.LogError($"Folder {folderPath} doesn't exist!");
            return;
        }
        var fileInfo = info.GetFiles("*.json", SearchOption.AllDirectories);
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

    void UnloadCard(string path)
    {
        throw new NotImplementedException();
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
        return Instance.cardsFromAll.TryGetValue(text, out value) ? value : new List<CardData>(0);
    }
    public static List<CardData> GetCardsTo(string text)
    {
        List<CardData> value;
        return Instance.cardsToAll.TryGetValue(text, out value) ? value : new List<CardData>(0);
    }

}