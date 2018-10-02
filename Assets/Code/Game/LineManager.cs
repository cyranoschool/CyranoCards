using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineManager : MonoBehaviour
{

    [Header("Init Prefabs")]
    public GameObject CardDropOff;
    public GameObject CardPickup;
    //[Header("Init")]

    [Header("Config")]
    public string LineString;
    public string LanguageFolder;
    public bool reloadCards = false;

    List<List<CardIndexer>> wordIndices;


    // Use this for initialization
    void Start()
    {

        if (reloadCards)
        {
            CardManager.LoadFolder(LanguageFolder);
        }
        //a b c b a c b c
        //1 2 3 2 1 3 2 3
        wordIndices = BuildWords(CardManager.Direction.To);
        PrintWordIndices();
    }

    void PrintWordIndices()
    {
        //Print wordIndices
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Line to break up: {LineString}");
        string separator = "";

        var split = LineString.Split();

        for (int i = 0; i < wordIndices.Count; i++)
        {
            sb.Append($"{split[i]}:{separator}".PadRight(20));

            var words = wordIndices[i];
            for (int j = 0; j < words.Count; j++)
            {
                sb.Append($"{words[j].Card.From}-->{words[j].Card.To}{separator}".PadRight(20));
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// Looks for cards in LanguageFolder that match the "from" words from the LineString
    /// </summary>
    /// <returns>Returns a list of length n being the number of words in LineString with elements being cards that can be put in these locations</returns>
    List<List<CardIndexer>> BuildWords(CardManager.Direction direction = CardManager.Direction.From)
    {
        string[] words = LineString.Split();

        //Each wordGroup is made up of cards with wordcount words.Length - i + 1
        //Each element keeps track where it starts with CardIndexer
        HashSet<CardIndexer> cardSet = new HashSet<CardIndexer>();

        //Search for largest words first and add them to list then get smaller words
        StringBuilder sb = new StringBuilder();
        for (int wordSize = words.Length; wordSize > 0; wordSize--)
        {
            //The group of all words of the same length within this line
            //List<CardIndexer> currentGroup = new List<CardIndexer>();
            //wordGroups.Add(currentGroup);
            for (int i = 0; i <= words.Length - wordSize; i++)
            {
                //Append all words together
                for (int j = 0; j < wordSize; j++)
                {
                    //Concatenate all words into single string to form a key
                    //e.g. "a" "b" "c" --> "a b c"
                    string word = words[i + j];
                    if (j > 0)
                    {
                        word = " " + word;
                    }
                    sb.Append(word);
                }
                string key = sb.ToString();
                sb.Clear();

                //Get all cards that match that key and create CardIndexers for them
                var set = CardManager.GetCards(key, direction);
                var cardIndexerSet = set.Select(x => new CardIndexer(x, i, wordSize));
                cardSet.UnionWith(cardIndexerSet);
            }
        }

        //Add children of cards as well
        //Requires a Hashset so there are no duplicate CardData
        //This is because a child may not be the same word as the lower level word
        //
        //The relationship between card children is still undecided

        //Populate output [index][n CardData that fits]
        List<List<CardIndexer>> slots = new List<List<CardIndexer>>(words.Length);
        for(int i = 0; i < words.Length; i++)
        {
            slots.Add(new List<CardIndexer>());
        }
        foreach(CardIndexer cardI in cardSet)
        {
            slots[cardI.Index].Add(cardI);
        }
        return slots;
    }

    class CardIndexer : IEqualityComparer<CardIndexer>
    {
        public CardData Card;
        public int Index;
        public int Length;

        public CardIndexer(CardData card, int index, int length)
        {
            this.Card = card;
            this.Index = index;
            this.Length = length;
        }

        //CardData are only considered the same if they have the same card AND same index
        public bool Equals(CardIndexer x, CardIndexer y)
        {
            //Any carddata should have the same length, so it doesn't need to be checked
            return x.Card == y.Card && x.Index == y.Index;
        }

        public int GetHashCode(CardIndexer obj)
        {
            return Card.GetHashCode() ^ Index;
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
