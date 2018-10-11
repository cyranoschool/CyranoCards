using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParentCard : MonoBehaviour {

    static ParentCard currentShowing;

    [Header("Init")]
    public List<GameObject> LineCards;

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
        LineCards.ForEach(x => x.SetActive(true));
    }

    public void HideCards()
    {
        LineCards.ForEach(x => x.SetActive(false));
    }
}
