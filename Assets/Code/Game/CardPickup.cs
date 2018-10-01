using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPickup : MonoBehaviour {

    public Transform HomeLocation;
    public Transform StartLocation;
    public bool StartMovingHome;
    public float Speed = 1f;
    private bool movingHome = false;

	// Use this for initialization
	void Start () {
        
        if(HomeLocation == null)
        {
            //Can be set somewhere other than spawn
            //So they can all fly to their homes at the beginning of the game
            var home = GameObject.Instantiate(new GameObject("CardHome"), transform.parent);
            home.transform.position = transform.position;
            HomeLocation = home.transform;
        }

        if (StartLocation != null)
        {
            transform.position = StartLocation.position;
        }
        movingHome = StartMovingHome;
        GetComponent<BoxCollider2D>().enabled = !movingHome;
    }
	
	// Update is called once per frame
	void Update () {
		if(movingHome)
        {
            transform.position = Vector3.Lerp(transform.position, HomeLocation.position, Speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, HomeLocation.position) <= .001f)
            {
                transform.position = HomeLocation.position;
                //Card is home
                movingHome = false;
                //Reenable trigger
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
	}

    public void MoveHome()
    {
        movingHome = true;
    }
}
