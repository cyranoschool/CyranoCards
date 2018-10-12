using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParentCard : MonoBehaviour {

    public enum Layer { Story = 4, Section = 3, Line = 2 }

    static Dictionary<int, ParentCard> currentShowing = new Dictionary<int, ParentCard>();

    public Transform HideTransform { get; set; }
    public int LayoutLayer { get; set; }
    Transform cardsLayout;

    [Header("Init")]
    public List<GameObject> LineCards = new List<GameObject>();

	// Use this for initialization
	void Start () {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { DragBegin((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void DragBegin(PointerEventData data)
    {
        //Upwards drag
        if(data.delta.y > 0)
        {
            HideLayer(LayoutLayer + 1);
        }
        else if(data.delta.y < 0)
        {
            ShowCards();
        }
    }

    public void ShowCards()
    {
        HideLayer(LayoutLayer);

        currentShowing[LayoutLayer] = this;

        foreach(GameObject card in LineCards)
        {
            CanvasGroup group = card.GetComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
            group.transform.localScale = Vector3.one;
            if (cardsLayout != null)
            {
                card.transform.SetParent(cardsLayout, false);
            }  
        }
    }

    public void HideLayer(int layerCheck)
    {
        //If a new story is clicked it should also hide cards from the other story's section + lines
        while (currentShowing.ContainsKey(layerCheck))
        {
            currentShowing[layerCheck].HideCards();
            currentShowing.Remove(layerCheck);
            layerCheck--;
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
            if (cardsLayout == null)
            {
                cardsLayout = card.transform.parent;
            }
            card.transform.SetParent(HideTransform, false);
        }
    }

    //Maintain static dictionary in cases like level loading
    void OnDestroy()
    {
        if(currentShowing.ContainsKey(LayoutLayer) && currentShowing[LayoutLayer] == this)
        {
            currentShowing.Remove(LayoutLayer);
        }
    }
}
