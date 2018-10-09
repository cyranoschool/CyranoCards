using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDropoff : MonoBehaviour
{

    public float TriggerScale = 1f;

    LineManager.CardIndexer cardIndexer;
    public LineManager.CardIndexer GetCardIndexer() { return cardIndexer; }

    CardManager.Direction direction;
    public CardManager.Direction GetDirection() { return direction; }

    GameObject uiBlock;
    public GameObject UIBlock { get { return uiBlock; } }

    TextMeshProUGUI uiTextMesh;
    public TextMeshProUGUI UITextMesh { get { return uiTextMesh; } }

    bool tookCard = false;
    public bool TookCard { get { return tookCard; } }

    //BoxCollider2D boxCollider2D;

    private void Awake()
    {
        //boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if (uiBlock != null)
        {
            //Works as long as in screen and not world space
            Camera cam = Camera.main;
            Transform childTransform = uiTextMesh.transform;
            Vector3 newPos = cam.ScreenToWorldPoint(childTransform.position);
            newPos.z = transform.parent.position.z;
            transform.position = newPos;

            //Workaround to get width of ui elements in worldspace
            //This took a lot of fiddling to figure out since there isn't a preckaged solution
            RectTransform rect = childTransform.GetComponent<RectTransform>();
            Rect r = RectTransformUtility.PixelAdjustRect(rect, GameObject.FindObjectOfType<Canvas>());
            //Convert from pixel to world
            float pixelsPerUnit = GameObject.FindObjectOfType<CanvasScaler>().referencePixelsPerUnit;

            transform.localScale = new Vector3(((r.width * TriggerScale) / pixelsPerUnit) / 2f, 1f, 1f);

        }
    }

    //Can be changed to allow a dropoff to take in multiple cards/parts
    public bool CanTakeCard()
    {
        return !tookCard;
    }

    public bool IsSolution(CardPickup pickupCard)
    {
        if(!CanTakeCard())
        {
            return false;
        }
        CardData card = cardIndexer.Card;
        CardData otherCard = pickupCard.GetCardIndexer().Card;
        if (direction == CardManager.Direction.To)
        {
            return card.To.Equals(otherCard.To);
        }
        else
        {
            return card.From.Equals(otherCard.From);
        }
    }

    public void GiveCard(Transform holding)
    {
        //Perform some kind of animation
        //Make some sort of success sound
        //Shoot out some sort of particles

        holding.SetParent(transform);

        //Make it move back to its home but this time in a new location
        CardPickup pickup = holding.GetComponent<CardPickup>();
        pickup.HomeLocation.position = transform.position + Vector3.down;

        pickup.MoveHome();
        tookCard = true;
    }

    public void SetCard(LineManager.CardIndexer cardIndexer, CardManager.Direction direction, GameObject uiBlock)
    {
        this.cardIndexer = cardIndexer;
        this.direction = direction;
        this.uiBlock = uiBlock;
        uiTextMesh = uiBlock.GetComponentInChildren<TextMeshProUGUI>();

        //Format text to fit a sentence
        CardData card = cardIndexer.Card;
        name = card.From + "-->" + card.To;
    }

}
