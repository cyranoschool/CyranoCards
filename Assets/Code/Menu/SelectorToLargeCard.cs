using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorToLargeCard : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LargeCard lCard = GetComponent<LargeCard>();
        CardSelectPasser passer = GameObject.FindObjectOfType<CardSelectPasser>();
        lCard.SetCard(passer.GetSelectedCard());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
