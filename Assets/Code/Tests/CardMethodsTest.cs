using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMethodsTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CardManager.Instance.LoadFolder("TestABC_Test123");
        foreach(string cardKey in CardManager.GetCardsFrom("a b c")[0].Children)
        {
            var card = CardManager.GetCardsFrom(cardKey);
            Debug.Log(card[0].From);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
