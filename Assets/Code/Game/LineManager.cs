using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

public class LineManager : MonoBehaviour
{

    [Header("Init Prefabs")]
    public GameObject CardDropOff;
    public GameObject CardPickup;
    public GameObject TextBlock;
    public Transform PickupParent;
    public Transform DropoffParent;
    public Transform BlockUI;
    //[Header("Init")]

    [Header("Config")]
    public string LineString;
    public string LanguageFolder;
    public CardManager.Direction direction = CardManager.Direction.To;
    public bool reloadCards = false;

    List<List<CardIndexer>> wordIndices;


    // Use this for initialization
    void Start()
    {

        if (reloadCards)
        {
            CardManager.LoadFolder(LanguageFolder);
        }
        //To: I ate three blind mice last night
        //From: yo comé trés ratoncitos ciegos anocho
        wordIndices = BuildWords(direction);

        ////a b c b a c b c
        //1 2 3 2 1 3 2 3
        PrintWordIndices();

        CreateCardGameElements(GetBestPhrase());

        
    }

    void CreateCardGameElements(List<CardIndexer> phrase)
    {

        //TEMPORARY setting of positions
        float dropoffSpacing = .5f;
        float lastOffset = 0;

        for (int i = 0; i < phrase.Count; i++)
        {
            CardIndexer cardIndexer = phrase[i];
            CardData card = cardIndexer.Card;
            CardPickup cardP = GameObject.Instantiate(CardPickup,PickupParent).GetComponent<CardPickup>();
            cardP.transform.position = PickupParent.position;
            //Pick card based on directionality
            cardP.SetCard(cardIndexer, direction);

            CardDropoff dropoff = GameObject.Instantiate(CardDropOff, DropoffParent).GetComponent<CardDropoff>();
            dropoff.transform.position = DropoffParent.position;

            //From or to length
            string textCard = direction == CardManager.Direction.To ? card.To : card.From;
            string textBlock = direction == CardManager.Direction.To ? card.From : card.To;

            //Temp String formatting for capital first letter and period at end
            if(i == 0)
            {
                if (!string.IsNullOrEmpty(textCard))
                {
                    textCard = textCard.First().ToString().ToUpper() + textCard.Substring(1);
                }
                if(!string.IsNullOrEmpty(textBlock))
                {
                    textBlock = textBlock.First().ToString().ToUpper() + textBlock.Substring(1);
                }
            }
            else if(i == phrase.Count - 1)
            {
                if (!string.IsNullOrEmpty(textCard))
                {
                    textCard += ".";
                }
                if (!string.IsNullOrEmpty(textBlock))
                {
                    textBlock += ".";
                }
            }

            GameObject uiText = GameObject.Instantiate(TextBlock, BlockUI);
            uiText.GetComponentInChildren<TextMeshProUGUI>().text = textBlock;

            dropoff.SetCard(cardIndexer, direction, uiText);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Line Processing
    /// <summary>
    /// Gets list of cards that
    /// A) Has the largest phrases left to right
    /// B) If there are multiple largest phrases sort by (favorited -> owner -> dateCreated)
    /// </summary>
    /// <returns></returns>
    List<CardIndexer> GetBestPhrase()
    {
        List<CardIndexer> cardPhrase = new List<CardIndexer>();
        var swipablePhrases = GetSwipablePhrases();
        
        for (int i = 0; i < swipablePhrases.Count; i++)
        {
            List<CardIndexer> cardList = swipablePhrases[i];
            //Sort by favorite, owned, whatever
            //Player saved card data not yet available
            //var sortedCards = cardList.OrderBy(x => x.Card.Favorited).ThenBy(x => x.Card.owner == myself).ThenBy( x => x.Card.owner).ThenBy( x => x.Card.creationDate);
            //cardPhrase.Add(sortedCards.First());

            CardIndexer card = cardList.Count > 0 ? cardList[0] : null;

            cardPhrase.Add(card);
        }
        
        return cardPhrase;
    }

    /// <summary>
    /// Gets list of cards that
    /// A) Has the largest phrases left to right
    /// B) Cards of the same size are grouped together (can be swiped through)
    /// </summary>
    /// <returns></returns>
    List<List<CardIndexer>> GetSwipablePhrases()
    {
        List<List<CardIndexer>> cardPhrase = new List<List<CardIndexer>>();
        for (int i = 0; i < wordIndices.Count; )
        {
            List<CardIndexer> cardPile = new List<CardIndexer>();
            cardPhrase.Add(cardPile);

            List<CardIndexer> words = wordIndices[i];
            if(words.Count == 0)
            {
                i++;
                continue;
            }
            //Sort descending by longest element
            words.Sort((x,y) => y.Length.CompareTo(x.Length));

            //Since list is sorted the largest length is the first element
            int length = words[0].Length;
            for (int j = 0; j < words.Count; j++)
            {
                CardIndexer cI = words[j];
                if(cI.Length == length)
                {
                    cardPile.Add(cI);
                }
                else
                {
                    break;
                }
            }
            i += length;
        }
        return cardPhrase;
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
                sb.Append($"{words[j].Card.From}-->{words[j].Card.To}{separator}".PadLeft(50));
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());

        //Print Best Phrase
        var phrase = GetBestPhrase();
        sb.Clear();

        //Innefficient, multiple loops, but it is just string output for testing on a very small amount of elements
        phrase.ForEach(card => sb.Append($"{card?.Card.From ?? "No Card"} "));
        sb.Append("-->");
        phrase.ForEach(card => sb.Append($"{card?.Card.To ?? "No Card"} "));
        sb.AppendLine();
        phrase.ForEach(card => sb.Append($"{card?.Card.From ?? "No Card"}-->{card?.Card.To ?? "No Card"}".PadRight(50)));
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
        //Can use a List<List<CardIndexer> to get GetSwipablePhrase and GetBestPhrase which can greatly improve search time
        //This works provided the conditions for GetSwipablePhrase doesn't ever change (breaking change)

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

        //Populate output [index][n CardData that fits]
        List<List<CardIndexer>> slots = new List<List<CardIndexer>>(words.Length);
        for (int i = 0; i < words.Length; i++)
        {
            slots.Add(new List<CardIndexer>());
        }
        foreach (CardIndexer cardI in cardSet)
        {
            slots[cardI.Index].Add(cardI);
        }
        
        return slots;
    }


    /// <summary>
    /// Stores a card and it's relationship in a line
    /// Also will contain comparisons for sorting cards (e.g. favorites more important than non-favorites)
    /// </summary>
    public class CardIndexer : IEqualityComparer<CardIndexer>
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
    #endregion
}
