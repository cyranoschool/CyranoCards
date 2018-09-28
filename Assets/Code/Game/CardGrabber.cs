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
            PickupCard card = c.GetComponent<PickupCard>();
            if (card == null)
            {
                return false;
            }

            //Put card into "hand" transform and disable its collider
            c.transform.SetParent(CardHolder);
            c.transform.localPosition = Vector3.zero;
            holding = c.transform;
            c.GetComponent<BoxCollider2D>().enabled = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    void TryDropCard(Collider2D c)
    {

    }
}
