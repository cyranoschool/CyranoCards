using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoryCard : MonoBehaviour {

    [Header("Init")]
    public Transform LineLayout;

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
