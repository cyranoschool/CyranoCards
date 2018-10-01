using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDropoff : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsSolution(CardPickup pickupCard)
    {
        return true;
    }

    public void GiveCard(Transform holding)
    {
        //Perform some kind of animation
        //Make some sort of success sound
        //Shoot out some sort of particles
        holding.SetParent(transform);
        holding.localPosition = Vector3.zero;
    }
}
