using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGrabber : MonoBehaviour
{

    [Header("Init")]
    public Transform CardHolder;
    public LargeCard largeCard;
    public LineManager lineManager;

    [Header("Config")]
    public Color32 SelectionColor = Color.yellow;
    public Color32 CompletionColor = new Color(0, 1f, 1f);
    public Color32 CompletionSelectionColor = new Color(0, .85f, .85f);

    Transform holding;
    CardDropoff wordCollider;
    Color32 textColor;
    bool didActionThisFrame = false;
    bool buttonPressed = false;
    bool buttonFixedPressed = false;

    public int TotalDrops { get; set; }
    public int IncorrectDrops { get; set; }
    public int CorrectDrops { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// Call to simulate a button press (useful for touch screen areas, or screen buttons)
    /// </summary>
    public void PressButton()
    {
        buttonPressed = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Button input has to be cached because it is checked in the fixed update which may or may not happen
        buttonPressed = buttonPressed | Input.GetButtonDown("Jump");

        //largeCard.gameObject.SetActive(holding != null);
        largeCard.SetVisible(holding != null);
    }

    private void FixedUpdate()
    {
        //Button press received
        buttonFixedPressed = buttonPressed;
        buttonPressed = false;
    }

    void LateUpdate()
    {
        //Move cardholder so it is always slightly in front of the parent
        //This needs to be done due to the object flipping
        Vector3 cardDepth = CardHolder.transform.position;
        cardDepth.z = CardHolder.parent.position.z - .05f;
        CardHolder.position = cardDepth;

        didActionThisFrame = false;
    }

    //Locks in first card dropoff collider player touches so only one trigger can be actively checked at once
    void OnTriggerEnter2D(Collider2D c)
    {
        OnTriggerStay2D(c);
    }

    //Release lock on a dropoff trigger
    void OnTriggerExit2D(Collider2D c)
    {
        TryReleaseTrigger(c);
    }

    void OnTriggerStay2D(Collider2D c)
    {
        TryLockInTrigger(c);
        //Don't keep trying to drop a card if you picked it up on this frame
        if (didActionThisFrame)
        {
            return;
        }
        if (!TryPickupCard(c))
        {
            TryDropCard(c);
        }
    }

    void TryLockInTrigger(Collider2D c)
    {
        CardDropoff dropOff = c.GetComponent<CardDropoff>();
        //Is this not a CardDropoff or does it already have a card that it took?
        if (dropOff == null)
        {
            return;
        }
        //Release old dropoff for new one if it is closer, otherwise ignore it
        else if (wordCollider != null)
        {
            if (Vector3.SqrMagnitude(transform.position - c.transform.position) < Vector3.SqrMagnitude(transform.position - wordCollider.transform.position))
            {
                TryReleaseTrigger(wordCollider.GetComponent<Collider2D>());
            }
            else
            {
                return;
            }
        }

        wordCollider = dropOff;
        //Change color of text if it is selectable
        var textMesh = dropOff.UITextMesh;
        textColor = textMesh.color;

        if(dropOff.CanTakeCard())
        {
            textMesh.color = SelectionColor;
        }
        else
        {
            textMesh.color = CompletionSelectionColor;
        }
    }

    void TryReleaseTrigger(Collider2D c)
    {
        CardDropoff dropOff = c.GetComponent<CardDropoff>();
        if (wordCollider == null || dropOff == null || wordCollider != dropOff)
        {
            return;
        }
        wordCollider = null;
        //Reset color of text
        var textMesh = dropOff.UITextMesh;
        textMesh.color = textColor;
    }

    bool TryPickupCard(Collider2D c)
    {
        if (holding == null && buttonFixedPressed)
        {
            CardPickup card = c.GetComponent<CardPickup>();
            if (card == null)
            {
                return false;
            }

            //Put card into "hand" transform and disable its collider
            c.transform.SetParent(CardHolder);
            c.transform.localPosition = Vector3.zero;
            holding = c.transform;
            //Collider can stay on if it is used for dropping
            c.GetComponent<BoxCollider2D>().enabled = false;
            didActionThisFrame = true;

            //Set large card to match this one
            largeCard.SetCard(card.GetCardIndexer().Card, false);
            largeCard.SetDirection(lineManager.Direction);

            return true;
        }
        else
        {
            return false;
        }
    }

    bool TryDropCard(Collider2D c)
    {
        //Only test for the first collider that player is touching
        CardDropoff dropOff = c.GetComponent<CardDropoff>();
        //Also ignore totally if dropoff already has all of its cards (but don't send card away)
        if (dropOff == null || dropOff != wordCollider || !dropOff.CanTakeCard())
        {
            return false;
        }


        if (holding && buttonFixedPressed)
        {
            //Increment drop counter
            TotalDrops += 1;

            //Check if correct card 
            //Unclear if card can be dropped if it's still wrong
            //To prevent backtracking it could fly back to where it came from
            CardPickup cardPickup = holding.GetComponent<CardPickup>();
            if (!dropOff.IsSolution(cardPickup))
            {
                IncorrectDrops += 1;
                //Send back to where it came from
                //Play some kind of negative sound
                AudioSource.PlayClipAtPoint(SoundManager.GetClip("ring_down"), transform.position);
                cardPickup.GetComponent<BoxCollider2D>().enabled = true;
                cardPickup.MoveHome();

                //Action was completed, no input should have more than one action happen
            }
            else
            {
                CorrectDrops += 1;
                //Remove CardPickup monobehaviour
                //Or set flag to prevent pickup again of card (dropoff may be one way)
                //Trigger is already disabled
                dropOff.GiveCard(holding);

                //Play positive sound
                AudioSource.PlayClipAtPoint(SoundManager.GetClip("ring_up"), transform.position);

                //Change color of text
                textColor  = CompletionColor;
                var textMesh = dropOff.UITextMesh;
                textMesh.color = CompletionSelectionColor;
            }
            
            holding = null;
            didActionThisFrame = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetDropAccuracy()
    {
        if(TotalDrops == 0)
        {
            return 0;
        }
        return (float)CorrectDrops / TotalDrops;
    }

}
