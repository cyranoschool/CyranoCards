using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropoff : MonoBehaviour {


    LineManager.CardIndexer cardIndexer;
    public LineManager.CardIndexer GetCardIndexer() { return cardIndexer; }
    CardManager.Direction direction;
    public CardManager.Direction GetDirection() { return direction; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsSolution(CardPickup pickupCard)
    {
        CardData card = cardIndexer.Card;
        CardData otherCard = pickupCard.GetCardIndexer().Card;
        if(direction == CardManager.Direction.To)
        {
            return card.To.Equals(otherCard.To);
        }
        else
        {
            return card.From.Equals(otherCard.From);
        }
    }

    public void GiveCard(Transform holding)
    {
        //Perform some kind of animation
        //Make some sort of success sound
        //Shoot out some sort of particles
        holding.SetParent(transform);
        holding.localPosition = Vector3.zero;
    }

    public void SetCard(LineManager.CardIndexer cardIndexer, CardManager.Direction direction)
    {
        this.cardIndexer = cardIndexer;
        this.direction = direction;

        CardData card = cardIndexer.Card;
        name = card.From + "-->" + card.To;
    }
}
