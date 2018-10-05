using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LargeCard : MonoBehaviour {

    [Header("Init")]
    public TextMeshProUGUI cardText;
    public Image image;


    private CardData cardData;
    public CardData GetCardData() { return cardData; }


    public CardManager.Direction direction = CardManager.Direction.From;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void SetCard(CardData card, bool forceUpdate = true)
    {
        cardData = card;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }
    public void SetDirection(CardManager.Direction dir, bool forceUpdate = true)
    {
        direction = dir;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }

    public void FlipDirection(bool forceUpdate = true)
    {
        direction = direction == CardManager.Direction.From ? CardManager.Direction.To : CardManager.Direction.From;
        if(forceUpdate)
        {
            UpdateCard();
        }
    }

    public void UpdateCard()
    {

        //Clear old data out
        //Not necessary in this implementation

        string text = direction == CardManager.Direction.From ? cardData.From : cardData.To;

        //Make first letter uppercase
        if (!string.IsNullOrEmpty(text))
        {
            text = text.First().ToString().ToUpper() + text.Substring(1);
        }

        cardText.text = text;

        //Do image setting here
        //
        //

    }
}
