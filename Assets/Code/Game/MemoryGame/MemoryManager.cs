using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoryManager : MonoBehaviour
{
    static MemoryManager Instance;

    [Header("Init")]
    public GameObject CardPrefab;
    [Header("Config")]
    public string CardType = "CardData";
    public float FlipBackDelay = .75f;

    CardData cardFocused;

    bool lockedSpin = false;
    public LargeCard selectedCard;

    void OnEnable()
    {
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }

    // Use this for initialization
    void Start()
    {
        CardSelectPasser passer = GameObject.FindObjectOfType<CardSelectPasser>();
        cardFocused = passer.GetSelectedCard();

        PopulateLayout();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PopulateLayout()
    {
        List<CardData> baseCards = cardFocused.GetChildCardsOfType(CardType);

        //Create card pairs
        foreach (CardData card in baseCards)
        {
            GameObject goFront = GameObject.Instantiate(CardPrefab, transform);
            GameObject goBack = GameObject.Instantiate(CardPrefab, transform);

            LargeCard lCardFront = goFront.GetComponent<LargeCard>();
            LargeCard lCardBack = goBack.GetComponent<LargeCard>();

            lCardFront.SetCard(card, false);
            lCardBack.SetCard(card, false);

            //Remember the card showing the front will hide the back and vice versa
            lCardFront.hideType = LargeCard.HideType.HideBack;
            lCardBack.hideType = LargeCard.HideType.HideFront;

            //Flip cards that are on the back
            lCardFront.FlipDirection(false);

            lCardFront.UpdateCard();
            lCardBack.UpdateCard();
        }

        //Take all cards out of transform, temporarily add to parent transform and then sort back in randomly
        List<Transform> childTransforms = new List<Transform>();
        foreach (Transform child in transform)
        {
            //So cards can be tapped and still dragged
            GameObject go = child.gameObject;
            go.AddComponent<DragPassthrough>();

            //Add click events
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { TappedCard((PointerEventData)data); });
            trigger.triggers.Add(entry);

            //Add to list of transforms to perform shuffle
            childTransforms.Add(child);
        }
        //Remove children to be added later
        transform.DetachChildren();

        //Shuffle list
        for (int i = 0; i < childTransforms.Count; i++)
        {
            int swapIndex = Random.Range(0, childTransforms.Count);
            Transform t = childTransforms[i];
            Transform swap = childTransforms[swapIndex];
            childTransforms[i] = swap;
            childTransforms[swapIndex] = t;
        }
        //Add list back into transform
        foreach (Transform child in childTransforms)
        {
            child.SetParent(transform);
        }

        SetFlippingChildren(true);
    }

    void TappedCard(PointerEventData data)
    {
        //Ignore these flip events
        if(lockedSpin)
        {
            return;
        }
        GameObject go = data.pointerCurrentRaycast.gameObject;
        LargeCard lCard = go.GetComponentInParent<LargeCard>();
        if(selectedCard == null)
        {
            selectedCard = lCard;
        }
        else if(selectedCard == lCard)
        {
            selectedCard = null;
        }
        else
        {
            //Selected the correct second card
            if(lCard.GetCardData() == selectedCard.GetCardData())
            {
                //Disable interactivity
                //Can't just set canspin because canspin is constantly reset (can make it only reset cards that aren't in solved hashset if necessary)
                CanvasGroup lCardGroup = lCard.GetComponent<CanvasGroup>();
                CanvasGroup selectedGroup = selectedCard.GetComponent<CanvasGroup>();
                lCardGroup.interactable = false;
                selectedGroup.interactable = false;
                lCardGroup.blocksRaycasts = false;
                selectedGroup.blocksRaycasts = false;

                //Show some other way that it was correct
                lCard.GetComponent<Image>().color = Color.cyan;
                selectedCard.GetComponent<Image>().color = Color.cyan;

                selectedCard = null;
            }
            else
            {

                //Make sure second card is still flipped due to how events are passed
                lCard.SpinCard();

                //Enforce a delay to see that you made a mistake
                SetFlippingChildren(false);
                StartCoroutine(FlipBackCards(lCard, selectedCard));
                //Wrong second card, lock out spinning of all the cards in order to show mistake
                selectedCard = null;
            }
        }
    }

    IEnumerator FlipBackCards(LargeCard a, LargeCard b)
    {
        yield return new WaitForSeconds(FlipBackDelay + a.SpinDuration);
        SetFlippingChildren(true);
        a.SpinCard();
        b.SpinCard();
    }

    void SetFlippingChildren(bool canFlip)
    {
        //Add list back into transform
        foreach (Transform child in transform)
        {
            child.GetComponent<LargeCard>().CanSpin = canFlip;
        }
        lockedSpin = !canFlip;
    }

    void ClearLayout()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void RestartGame()
    {
        ClearLayout();
        PopulateLayout();

    }
}
