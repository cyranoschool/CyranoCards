using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChildSelector : MonoBehaviour {

    //[Header("Init")]
    //Can include reference to the buttons to selectively hide them if there are 1 or less children
    //Buttons

    LargeCard largeCard;
	// Use this for initialization
	void Start () {
        largeCard = GetComponentInParent<LargeCard>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SelectNextChildLeft()
    {
        SelectNextChild(false);
    }
    public void SelectNextChildRight()
    {
        SelectNextChild(true);
    }

    void SelectNextChild(bool right)
    {
        List<CardData> childCards = largeCard.GetCardData().GetChildCards();
        if(childCards.Count == 0)
        {
            return;
        }
        CardData cardData = largeCard.GetCardData();
        int index = cardData.ChildCardViewIndex;
        //If right move forward, if left go backward in list of child cards
        int inc = right ? 1 : -1;
        index += inc;
        //negative 1 defaults the image to the default one for the card
        if (index == childCards.Count)
        {
            index = -1;
        }
        else if(index < -1)
        {
            index = childCards.Count - 1;
        }
        cardData.ChildCardViewIndex = index;
        largeCard.UpdateCard();

        //Save card/duplicate card here
        //
    }
}
