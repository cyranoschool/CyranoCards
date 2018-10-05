using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickup : MonoBehaviour {

    public Transform HomeLocation;
    public Transform StartLocation;
    public bool StartMovingHome;
    public float Speed = 1f;
    public float SpinSpeed = 8f;
    public Vector2 SpawnRangeX = new Vector2(-3.5f, 1f);
    public Vector2 SpawnRangeY = new Vector2(-.75f, .5f);

    LineManager.CardIndexer cardIndexer;
    public LineManager.CardIndexer GetCardIndexer() { return cardIndexer; }
    CardManager.Direction direction;
    public CardManager.Direction GetDirection() { return direction; }
    

    bool movingHome = false;
    bool wasEnabled = true;
    Vector3 originalScale;
	// Use this for initialization
	void Start () {
        originalScale = transform.localScale;
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
            float distance = Vector3.Distance(transform.position, HomeLocation.position);

            //Distance card will lock back in place (because lerping never technically reaches its goal)
            const float STOP_DISTANCE = .025f;

            //Flip card around when moving
            float newXScale = Mathf.Sin(distance * SpinSpeed + Mathf.PI *.5f + STOP_DISTANCE) * originalScale.x;

            //Deparent to set absolute scale
            Transform parent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(newXScale, originalScale.y, originalScale.z);

            if (distance <= STOP_DISTANCE)
            {
                transform.position = HomeLocation.position;
                transform.localScale = originalScale;
                //Card is home
                movingHome = false;
                //Reenable trigger
                GetComponent<BoxCollider2D>().enabled = wasEnabled;
            }
            transform.parent = parent;
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
        Vector3 randomSpot = new Vector3(UnityEngine.Random.Range(SpawnRangeX.x,SpawnRangeX.y),UnityEngine.Random.Range(SpawnRangeY.x, SpawnRangeY.y));
        HomeLocation.transform.position += randomSpot;
    }


    public void MoveHome()
    {
        wasEnabled = GetComponent<BoxCollider2D>().enabled;
        originalScale = transform.lossyScale;
        transform.parent = HomeLocation.transform.parent;
        movingHome = true;
    }
}
