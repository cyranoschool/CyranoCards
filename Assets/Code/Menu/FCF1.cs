using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class FCF1 : MonoBehaviour
{
    [Header("Init")]
    public GameObject CardPrefab, mainCard;
    public Text txtMessage;
    public List<GameObject> gridCards;

    [Header("Config")]
    public string CardType = "CardData";
    
    int NUMHANDCARDS = 4, QUITCOUNT = 100;
    List<CardData> baseCards;
    CardData cardFocused;
    
    // Use this for initialization
    void Start()
    {
        CardSelectPasser passer = GameObject.FindObjectOfType<CardSelectPasser>();
        cardFocused = passer.GetSelectedCard();
       
        PopulateLayout();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GridCardChanges(GameObject gridCard)
    {
        gridCard.GetComponent<LargeCard>().FlipDirection(true);
        gridCard.GetComponent<LargeCard>().CanSpin = false;
        gridCard.GetComponent<LargeCard>().cardText.text = "";

        EventTrigger trigger = gridCard.GetComponent<LargeCard>().GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { clickGridCard((PointerEventData)data, gridCard); });
        trigger.triggers.Add(entry);

    }

    void PopulateLayout()
    {
        baseCards = cardFocused.GetChildCardsOfType(CardType);
        //foreach (CardData card in baseCards)
        CardData card = baseCards[(int)UnityEngine.Random.Range(0f, baseCards.Count)];
        gridCards[0].GetComponent<LargeCard>().SetCard(card);
        GridCardChanges(gridCards[0]);
        mainCard.GetComponent<LargeCard>().SetCard(card);
      
        //So cards can be tapped and still dragged
        mainCard.AddComponent<DragPassthrough>();

        fillCurrentHandWithCards();

    }

    public void fillCurrentHandWithCards()
    {
        
        int count = 0;
        int gridCardCount = 1;
        while (gridCardCount < NUMHANDCARDS && count < QUITCOUNT)
        {
            CardData tempCardData = baseCards[(int)UnityEngine.Random.Range(0, baseCards.Count)];
        
            gridCards[gridCardCount].GetComponent<LargeCard>().SetCard(tempCardData);
            GridCardChanges(gridCards[gridCardCount]);  
            gridCardCount++;
            
            count++;
           
        }

        scrambleGridCards();
        
    }

    public void clickGridCard(PointerEventData data, GameObject clickedCard)
    {
        Debug.Log(clickedCard.GetComponent<LargeCard>().GetCardData().ToString() + " Attempt! "+ mainCard.GetComponent<LargeCard>().GetCardData().ToString());
        if (clickedCard.GetComponent<LargeCard>().GetCardData().To == mainCard.GetComponent<LargeCard>().GetCardData().To
            && clickedCard.GetComponent<LargeCard>().GetCardData().From == mainCard.GetComponent<LargeCard>().GetCardData().From)
        {
            clickedCard.GetComponent<LargeCard>().CanSpin = true;
            clickedCard.GetComponent<LargeCard>().SpinCard();
            txtMessage.text = "Correct!";
            
        }
            

    }


    public List<CardData> scrambleCardData(List<CardData> list)
    {
        Debug.Log("Xx");
        List<CardData> cD = new List<CardData>(list.Count);
        int cdCount = 0;

        while (cdCount != list.Count)
        {
            int i = (int)UnityEngine.Random.Range(0, list.Count);
            if (list[i] != null)
            {
                CardData newCD = list[i].Duplicate();
                cD.Add(newCD);//[cdCount] = newCD;
                cdCount++;
                list[i] = null;
            }
        }
        
        return cD;
    }

    public void scrambleGridCards()
    {
        // gridCards[3].GetComponent<LargeCard>().SetCard(gridCards[0].GetComponent<LargeCard>().GetCardData());
        Debug.Log("$$$$$$$$$$$$$"+gridCards.Count);

        List<CardData> cD = new List<CardData>(gridCards.Count);
        
        for (int i = 0; i < gridCards.Count; i++)
        {
            cD.Add(gridCards[i].GetComponent<LargeCard>().GetCardData());
            Debug.Log("" + gridCards[i].GetComponent<LargeCard>().GetCardData());
        }

       
        cD = scrambleCardData(cD);

        for (int i = 0; i < gridCards.Count; i++)
           gridCards[i].GetComponent<LargeCard>().SetCard(cD[i]);
            
    }



    public bool isInList(List<GameObject> cards, CardData card)
    {
        bool isIn = false;

        for (int i=0; i<cards.Count;i++)
        {
            if (cards[i].GetComponent<LargeCard>().GetCardData() != card)
                if (cards[i].GetComponent<LargeCard>().GetCardData().From.ToUpper() == card.From.ToUpper() && cards[i].GetComponent<LargeCard>().GetCardData().To.ToUpper() == card.To.ToUpper())
                    isIn = true;
        }

        return isIn;
    }

}
