using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropoff : MonoBehaviour {


    LineManager.CardIndexer cardIndexer;
    public LineManager.CardIndexer GetCardIndexer() { return cardIndexer; }
    CardManager.Direction direction;
    public CardManager.Direction GetDirection() { return direction; }
    GameObject uiBlock;
    public GameObject GetUIBlock() { return uiBlock; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        if(uiBlock != null)
        {
            //Works as long as in screen and not world space
            Vector3 newPos = Camera.allCameras[0].ScreenToWorldPoint(uiBlock.transform.position);
            newPos.z = transform.parent.position.z;
            transform.position = newPos;

            //Currently working on getting the proper width of the collider
            //var bounds = RectTransformUtility.sc(uiBlock.transform);
            //GetComponent<BoxCollider2D>().size = (Vector2)bounds.size;
            //uiBlock.GetComponent<RectTransform>().
            //RectTransformUtility
        }
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

    public void SetCard(LineManager.CardIndexer cardIndexer, CardManager.Direction direction, GameObject uiBlock)
    {
        this.cardIndexer = cardIndexer;
        this.direction = direction;
        this.uiBlock = uiBlock;

        CardData card = cardIndexer.Card;
        name = card.From + "-->" + card.To;
    }
}
