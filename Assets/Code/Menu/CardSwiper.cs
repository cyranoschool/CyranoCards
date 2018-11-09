using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSwiper : MonoBehaviour {

    //CardStack
    //Iterate through stack
    //Card stack is retrieved through CardManager?
    /*var sortedCards = cardList.OrderBy(x => Card pinned?)
                                .ThenBy(x => x.Card.Favorited)
                                .ThenBy(x => x.Card.owner == myself)
                                .ThenBy( x => x.Card.owner)
                                .ThenBy( x => x.Card.creationDate);
    */

    // Use this for initialization
    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((data) => { DragBegin((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public void DragBegin(PointerEventData data)
    {
        float deltaXAbs = Mathf.Abs(data.delta.x);
        float deltaYAbs = Mathf.Abs(data.delta.y);
        
        if (deltaYAbs > deltaXAbs)
        {
            //Upwards drag
            if (data.delta.y > 0)
            {
                //Debug.Log("DragUp");
            }
            //Downwards drag
            else if (data.delta.y < 0)
            {
                //Debug.Log("DragDown");
            }
        }
    }

    
}
