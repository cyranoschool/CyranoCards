using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickup : MonoBehaviour {

    public Transform HomeLocation;
    public Transform StartLocation;
    public bool StartMovingHome;
    public float Speed = 1f;

    LineManager.CardIndexer cardIndexer;
    public LineManager.CardIndexer GetCardIndexer() { return cardIndexer; }
    CardManager.Direction direction;
    public CardManager.Direction GetDirection() { return direction; }
    

    bool movingHome = false;
    

	// Use this for initialization
	void Start () {

        CreateHomeLocation();

        if (StartLocation != null)
        {
            transform.position = StartLocation.position;
        }
        movingHome = StartMovingHome;
        GetComponent<BoxCollider2D>().enabled = !movingHome;
    }
	
    void CreateHomeLocation()
    {
        if (HomeLocation == null)
        {
            //Can be set somewhere other than spawn
            //So they can all fly to their homes at the beginning of the game
            var home = new GameObject("CardHome: " + name);
            home.transform.parent = transform.parent;
            home.transform.position = transform.position;
            HomeLocation = home.transform;
        }
    }

	// Update is called once per frame
	void Update () {
		if(movingHome)
        {
            transform.position = Vector3.Lerp(transform.position, HomeLocation.position, Speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, HomeLocation.position) <= .01f)
            {
                transform.position = HomeLocation.position;
                //Card is home
                movingHome = false;
                //Reenable trigger
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
	}

    public void SetCard(LineManager.CardIndexer cardIndexer, CardManager.Direction direction)
    {
        this.cardIndexer = cardIndexer;
        this.direction = direction;

        CardData card = cardIndexer.Card;
        name = card.From + "-->" + card.To;

        //TEMPORARY setting location to somewhere random;
        CreateHomeLocation();
        Vector3 randomSpot = new Vector3(UnityEngine.Random.Range(-3.5f, 3.5f),UnityEngine.Random.Range(-.75f, .5f));
        HomeLocation.transform.position += randomSpot;
    }


    public void MoveHome()
    {
        transform.parent = HomeLocation.transform.parent;
        movingHome = true;
    }
}
