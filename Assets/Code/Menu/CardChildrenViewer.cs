using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardChildrenViewer : MonoBehaviour {
    [Header("Init")]
    public GameObject CardPrefab;

    CardData cardFocused;

	// Use this for initialization
	void Start () {
        CardSelectPasser passer = GameObject.FindObjectOfType<CardSelectPasser>();
        cardFocused = passer.GetSelectedCard();

        PopulateLayout();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void PopulateLayout()
    {
        List<CardData> baseCards = cardFocused.GetBaseWordCards();
        foreach(CardData card in baseCards)
        {
            GameObject go = GameObject.Instantiate(CardPrefab, transform);
            go.GetComponent<LargeCard>().SetCard(card);
            //So cards can be tapped and still dragged
            go.AddComponent<DragPassthrough>();
        }
    }
}
