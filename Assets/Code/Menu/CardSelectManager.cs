using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Adds click events to cards in scene so that they can be selected to move to another scene
/// </summary>
public class CardSelectManager : MonoBehaviour {

    HashSet<LargeCard> selectedCards = new HashSet<LargeCard>();
    Color previousColor;

	// Use this for initialization
	void Start () {
       //Call after start
       Invoke("FindCardsAddTriggers",0);
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
        if(data.clickCount < 2)
        {
            return;
        }
        GameObject go = data.pointerCurrentRaycast.gameObject;
        LargeCard selected = go.GetComponentInParent<LargeCard>();
        
        if(!selectedCards.Contains(selected))
        {
            SelectCard(selected);
        }
        else
        {
            UnselectCard(selected);
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
	void Update () {
		
	}
}
