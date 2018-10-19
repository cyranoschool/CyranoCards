using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Adds click events to cards in scene so that they can be selected to move to another scene
/// </summary>
public class CardSelectManager : MonoBehaviour
{

    //Multiple cards can be passed on but for this purpose only one is needed
    public int MaxSelectedCards = 1;

    HashSet<LargeCard> selectedCards = new HashSet<LargeCard>();
    Queue<LargeCard> cardQueue = new Queue<LargeCard>();
    Color previousColor;

    // Use this for initialization
    void Start()
    {
        //Call after start
        //The triggers will have to be added if new cards are ever added to the groups
        //in this case there should be a delegate that can be subscribed to on LargeCard creation there
        Invoke("FindCardsAddTriggers", 0);
    }

    void FindCardsAddTriggers()
    {
        //Get every card in scene and add trigger to them
        LargeCard[] cards = GameObject.FindObjectsOfType<LargeCard>();
        for (int i = 0; i < cards.Length; i++)
        {
            LargeCard lCard = cards[i];
            EventTrigger trigger = lCard.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { TappedCard((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }
    }

    void TappedCard(PointerEventData data)
    {
        //Have to double click
        if (data.clickCount < 2)
        {
            return;
        }
        GameObject go = data.pointerCurrentRaycast.gameObject;
        LargeCard selected = go.GetComponentInParent<LargeCard>();

        //Quick select and pass to scene as this is the only card that can be selected
        if (MaxSelectedCards == 1)
        {
            selectedCards.Add(selected);
            SendCardsToScene();
            return;
        }

        if (!selectedCards.Contains(selected))
        {
            SelectCard(selected);
            cardQueue.Enqueue(selected);
            while (cardQueue.Count > MaxSelectedCards)
            {
                LargeCard card = cardQueue.Dequeue();
                UnselectCard(card);
            }
        }
        else
        {
            UnselectCard(selected);
            //Remove this card from the queue
            Queue<LargeCard> newQueue = new Queue<LargeCard>();
            while (cardQueue.Count > 0)
            {
                LargeCard card = cardQueue.Dequeue();
                if (card != selected)
                {
                    newQueue.Enqueue(card);
                }
            }
            cardQueue = newQueue;
        }

    }

    //Somehow denote some kind of selection
    void SelectCard(LargeCard c)
    {
        Image image = c.GetComponent<Image>();
        previousColor = image.color;

        image.color = new Color(0, .5f, .5f);

        selectedCards.Add(c);
    }

    //Undo selection change
    void UnselectCard(LargeCard c)
    {
        Image image = c.GetComponent<Image>();
        image.color = previousColor;

        selectedCards.Remove(c);
    }

    // Update is called once per frame
    void Update()
    {
        //Temporary key input as there is no button yet
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendCardsToScene();
        }
    }

    void SendCardsToScene()
    {
        CardSelectPasser passer = GameObject.FindObjectOfType<CardSelectPasser>();
        if (passer == null)
        {
            //Create Scene Data passer if it doesn't exist
            GameObject go = new GameObject();
            passer = go.AddComponent<CardSelectPasser>();
        }

        //Add data to passer
        passer.Setup(selectedCards);
        passer.LevelTarget = "CardFocus";
        passer.DestroyAfterLoad = false;
        passer.DestroyOnWrongLevel = false;
        SceneManager.LoadScene("CardFocus");
    }

}

public class CardSelectPasser : SceneDataPasser
{
    List<CardData> cards = new List<CardData>();
    string Folder;
    SerializationManager.SavePathType PathType;

    public CardData GetSelectedCard()
    {
        return cards[0];
    }

    public void Setup(HashSet<LargeCard> largeCards)
    {
        cards.Clear();

        foreach (LargeCard lCard in largeCards)
        {
            cards.Add(lCard.GetCardData());
        }
        //Need refence of the user/language folder that this card came from
        MenuTreeGenerator generator = GameObject.FindObjectOfType<MenuTreeGenerator>();
        Folder = generator.StoryFolder;
        PathType = generator.PathType;
    }

    protected override void DoAfterLoad()
    {
        base.DoAfterLoad();
        //Set card in scene to this card data
        LargeCard lCard = GameObject.FindObjectOfType<LargeCard>();
        lCard.SetCard(cards[0]);

        //Assume card already has game line trigger
        GameLineTrigger trigger = lCard.GetComponent<GameLineTrigger>();
        trigger.PathType = PathType;
        trigger.Folder = Folder;
    }
}
