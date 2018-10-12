using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class LargeCard : MonoBehaviour
{

    [Header("Init")]
    public TextMeshProUGUI cardText;
    public Image image;


    [Header("Config")]
    public float SpinSpeed = 4f;
    public float SpinDuration = .25f;
    public bool CanSpin = true;

    private CardData cardData;
    public CardData GetCardData() { return cardData; }

    private CardManager.Direction direction = CardManager.Direction.From;
    public CardManager.Direction GetDirection() { return direction; }


    bool spinning = false;
    bool visible = true;

    // Use this for initialization
    void Start()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { TappedCard((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetCard(CardData card, bool forceUpdate = true)
    {
        cardData = card;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }

    public void SetVisible(bool setVisible)
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (setVisible)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        visible = setVisible;
    }

    public void SetDirection(CardManager.Direction dir, bool forceUpdate = true)
    {
        direction = dir;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }

    public void FlipDirection(bool forceUpdate = true)
    {
        direction = direction == CardManager.Direction.From ? CardManager.Direction.To : CardManager.Direction.From;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }

    public void UpdateCard()
    {
        if (cardData == null)
        {
            return;
        }
        //Clear old data out
        //Not necessary in this implementation

        string text = direction == CardManager.Direction.From ? cardData.From : cardData.To;

        //Make first letter uppercase
        if (!string.IsNullOrEmpty(text))
        {
            text = text.First().ToString().ToUpper() + text.Substring(1);
        }

        cardText.text = text;

        //Do image setting here
        //
        //

    }

    public void TappedCard(PointerEventData data)
    {
        //Ignore if this isn't a true click (was doing a drag and released)
        if (data.dragging)
        {
            return;
        }
        //Don't spin it twice, wait for spinning to finish
        if (!CanSpin || spinning)
        {
            return;
        }
        FlipDirection();
        StartCoroutine(Spin(SpinSpeed, SpinDuration, transform.localScale));
    }

    IEnumerator Spin(float speed, float duration, Vector3 originalScale)
    {
        SoundManager.GetSound("flick").Play();
        spinning = true;
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            float xScale = Mathf.Sin(t * speed) * originalScale.x;
            transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
            yield return null;
        }
        spinning = false;
        transform.localScale = originalScale;
    }

}
