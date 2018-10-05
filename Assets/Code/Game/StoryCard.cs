using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCard : MonoBehaviour {

    [Header("Init")]
    public Transform LineLayout;

    public List<GameObject> LineCards;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowCards()
    {
        for(int i = 0; i < LineLayout.childCount; i++)
        {
            LineLayout.GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i < LineCards.Count; i++)
        {
            LineCards[i].gameObject.SetActive(true);
        }
    }
}
