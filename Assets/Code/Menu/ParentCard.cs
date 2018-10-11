using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParentCard : MonoBehaviour {

    static ParentCard currentShowing;

    [Header("Init")]
    public List<GameObject> LineCards = new List<GameObject>();

	// Use this for initialization
	void Start () {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(ShowCards);
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ShowCards(BaseEventData arg0)
    {
        if(currentShowing != null)
        {
            currentShowing.HideCards();
        }
        currentShowing = this;
        foreach(GameObject card in LineCards)
        {
            CanvasGroup group = card.GetComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }

    public void HideCards()
    {
        foreach (GameObject card in LineCards)
        {
            CanvasGroup group = card.GetComponent<CanvasGroup>();
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}
