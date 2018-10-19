using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipAllCardsButton : MonoBehaviour {

    public List<Transform> CardParents;
    [Header("Config")]
    public bool DeepSearch = false;

    CardManager.Direction direction = CardManager.Direction.From;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FlipAll()
    {
        foreach(Transform t in CardParents)
        {
            foreach (LargeCard card in t.GetComponentsInChildren<LargeCard>())
            {
                //Only spin cards that are facing the wrong way
                if(card.GetDirection() == direction)
                {
                    card.SpinCard();
                }
            }
        }
        //Flip direction
        direction = (direction == CardManager.Direction.From) ? CardManager.Direction.To : CardManager.Direction.From;
    }
}
