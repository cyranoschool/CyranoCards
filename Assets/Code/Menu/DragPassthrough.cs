using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPassthrough : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    ScrollRect scrollParent;
    // Use this for initialization
    void Start()
    {
        scrollParent = GetComponentInParent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollParent.SendMessage("OnBeginDrag", eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollParent.SendMessage("OnDrag", eventData);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        scrollParent.SendMessage("OnEndDrag", eventData);
    }
}
