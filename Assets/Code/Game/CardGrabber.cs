using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGrabber : MonoBehaviour {

    [Header("Init")]
    public Transform CardHolder;

    private Transform holding;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
        //Move cardholder so it is always slightly in front of the parent
        //This needs to be done due to the object flipping
        Vector3 cardDepth = CardHolder.transform.position;
        cardDepth.z = CardHolder.parent.position.z - .05f;
        CardHolder.position = cardDepth;

    }

    void OnTriggerStay2D(Collider2D c)
    {
        if(!TryPickupCard(c))
        {
            TryDropCard(c);
        }
    }

    bool TryPickupCard(Collider2D c)
    {
        if (holding == null && Input.GetButtonDown("Jump"))
        {
            CardPickup card = c.GetComponent<CardPickup>();
            if (card == null)
            {
                return false;
            }

            //Put card into "hand" transform and disable its collider
            c.transform.SetParent(CardHolder);
            c.transform.localPosition = Vector3.zero;
            holding = c.transform;
            //Collider can stay on if it is used for dropping
            c.GetComponent<BoxCollider2D>().enabled = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TryDropCard(Collider2D c)
    {
        if(holding && Input.GetButtonDown("Jump"))
        {
            CardDropoff dropOff = c.GetComponent<CardDropoff>();
            if (dropOff == null)
            {
                return false;
            }
            //Check if correct card 
            //Unclear if card can be dropped if it's still wrong
            //To prevent backtracking it could fly back to where it came from
            CardPickup cardPickup = holding.GetComponent<CardPickup>();
            if (!dropOff.IsSolution(cardPickup))
            {
                //Send back to where it came from
                //Play some kind of negative sound
                cardPickup.MoveHome();

                //Action was completed, no input should have more than one action happen
            }
            else
            {
                //Remove CardPickup monobehaviour
                //Or set flag to prevent pickup again of card (dropoff may be one way)
                //Trigger is already disabled
                dropOff.GiveCard(holding);
            }
            
            holding = null;

            return true;
        }
        else
        {
            return false;
        }
    }
}
