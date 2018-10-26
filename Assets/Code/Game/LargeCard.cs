using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System.IO;

public class LargeCard : MonoBehaviour
{

    [Header("Init")]
    public TextMeshProUGUI cardText;
    public TextMeshProUGUI phoneticText;
    public Image image;
    public GameObject BackOverlay;
    public Toggle FavoriteToggle;
    public Image BadgeImage;

    [Header("Config")]
    public string fallbackImage = "questionmark";
    public float SpinSpeed = 4f;
    public float SpinDuration = .25f;
    public bool CanSpin = true;
    public Color FrontColor;
    public Color BackColor;

    public enum HideType { NoHide, HideFront, HideBack, HideBoth }
    public HideType hideType = HideType.NoHide;

    //Are all children images checked, just the ones from the child selected, or none at all
    public enum ImageChecking { None, Selected, All }
    public ImageChecking imageChecking = ImageChecking.All;

    private CardData cardData;
    public CardData GetCardData() { return cardData; }

    private CardManager.Direction direction = CardManager.Direction.From;
    public CardManager.Direction GetDirection() { return direction; }

    bool spinning = false;
    bool visible = true;
    public bool Visible { get { return visible; } }

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
        direction = (direction == CardManager.Direction.From) ? CardManager.Direction.To : CardManager.Direction.From;
        if (forceUpdate)
        {
            UpdateCard();
        }
    }
    public static List<string> GetPotentialImageNames(CardData cd)
    {
        return new List<string>() { cd.Icon, cd.From, cd.PhoneticFrom, cd.BrokenUpTo, cd.To };
    }

    /// <summary>
    /// This gets called frequently when card is changed in some way, or interacted with
    /// This method should eventually be swapped out for an event that other scripts subscribe to
    /// An event based UpdateCard would allow for scalability (maintainability/single responsibility) and modularity
    /// </summary>
    public void UpdateCard()
    {
        if (cardData == null)
        {
            return;
        }

        //Clear old data out
        //Primary text is always From now
        //string text = direction == CardManager.Direction.From ? cardData.From : cardData.To;
        string text = cardData.From;

        //Make first letter uppercase
        if (!string.IsNullOrEmpty(text))
        {
            text = text.First().ToString().ToUpper() + text.Substring(1);
        }

        cardText.SetText(text);

        //Do image setting here
        UpdateImage();

        //Changes here, phonetic text is always active but is swapped for the broken up text
        //If direction is from then set pronounceText
        if (direction == CardManager.Direction.From)
        {
            //GameObject parentGameObject = phoneticText.transform.parent.gameObject;
            //parentGameObject.gameObject.SetActive(true);
            phoneticText.SetText(cardData.PhoneticFrom);
        }
        else
        {
            //GameObject parentGameObject = phoneticText.transform.parent.gameObject;
            //parentGameObject.SetActive(false);
            phoneticText.SetText(cardData.To);
        }

        //Card backing
        UpdateBacking();

        //Update Favorite toggle
        FavoriteToggle.isOn = cardData.IsFavorited();

        Image panelImage = GetComponent<Image>();
        //Update color of card (front or back)
        if (direction == CardManager.Direction.From)
        {
            panelImage.color = FrontColor;
        }
        else
        {
            panelImage.color = BackColor;
        }

        //Update progress bar
        UpdateProgress();

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        //Canvas.ForceUpdateCanvases();

    }

    void UpdateImage()
    {
        List<CardData> childrenCards = cardData.GetChildCards();
        List<string> imageNames = GetPotentialImageNames(cardData);

        if (imageChecking == ImageChecking.Selected)
        {
            //Add strings of the selected child card index if it exists
            int childIndex = cardData.ChildCardViewIndex;
            if (childIndex > 0 && childIndex < cardData.ChildCardCount())
            {
                imageNames.AddRange(GetPotentialImageNames(childrenCards[childIndex]));
            }
        }
        else if (imageChecking == ImageChecking.All)
        {
            //Alternatively add all possible image names for all children
            //THIS COULD TAKE MUCH LONGER
            foreach (CardData c in childrenCards)
            {
                imageNames.AddRange(GetPotentialImageNames(c));
            }
        }

        imageNames.Add(fallbackImage);
        bool imageSet = false;
        for (int i = 0; i < imageNames.Count; i++)
        {
            if (TryLoadImage(imageNames[i]))
            {
                imageSet = true;
                break;
            }
        }
        //Set default image or leave alone
        if (!imageSet)
        {

        }
    }

    void UpdateBacking()
    {
        BackOverlay.SetActive(IsHidden());
    }
    bool TryLoadImage(string name)
    {
        string imageFolderPath = "Images/";
        Texture2D texture = Resources.Load<Texture2D>(imageFolderPath + name);
        if (texture == null)
        {
            return false;
        }
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));

        //Destroy old sprite here/unload
        //

        image.sprite = sprite;
        image.preserveAspect = true;

        return true;
    }

    void UpdateProgress()
    {
        UserData userData = UserManager.Instance?.GetCurrentUser();
        if (userData == null)
        {
            return;
        }
        //Get local data but DO NOT place in dictionary if it doesn't yet exist
        //It can be worthwhile not to save useless default save file data that is only a get
        var localData = userData.GetOrCreateLocalCardData(cardData.UID, false);
        int progress = localData.Progress;
        //Temporary value for max progress as the way progress works is still somewhat undefined
        int maxProgress = LocalUserCardData.MAX_PROGRESS;

        //Base bar progress/badge color/number of stars on progress ratio
        //These numbers are completely made up, they should be based on something concrete
        Color badgeColor;
        if (progress <= 0)
        {
            badgeColor = Color.red;
        }
        else if(progress < maxProgress)
        {
            badgeColor = Color.yellow;
        }
        else
        {
            badgeColor = Color.green;
        }
        BadgeImage.color = badgeColor;
    }

    public void TappedCard(PointerEventData data)
    {
        //Ignore if this isn't a true click (was doing a drag and released)
        if (data.dragging)
        {
            return;
        }
        SpinCard();
    }

    public void SpinCard()
    {
        //Don't spin it twice, wait for spinning to finish
        if (!CanSpin || spinning)
        {
            return;
        }
        FlipDirection();
        spinning = true;
        StartCoroutine(Spin(SpinSpeed, SpinDuration, transform.localScale));
    }

    IEnumerator Spin(float speed, float duration, Vector3 originalScale)
    {

        //Sound too short to play with playclipatpoint
        //AudioSource.PlayClipAtPoint(SoundManager.GetClip("flick"), transform.position);
        AudioSource flickSound = SoundManager.GetSound("flick");
        flickSound.Play();
        //Have to manually activate because sound is too short
        flickSound.GetComponent<SoundDestroyer>().activated = true;


        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            float xScale = Mathf.Sin(t * speed) * originalScale.x;
            transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
            yield return null;
        }
        spinning = false;
        transform.localScale = originalScale;
    }

    public bool IsHidden()
    {
        bool hidden = false;
        switch (hideType)
        {
            case HideType.NoHide:
                break;
            case HideType.HideFront:
                hidden = (direction == CardManager.Direction.From) ? true : false;
                break;
            case HideType.HideBack:
                hidden = (direction == CardManager.Direction.From) ? false : true;
                break;
            case HideType.HideBoth:
                hidden = true;
                break;
        }
        return hidden;
    }

    public void ToggleFavorite(bool IsOn)
    {
        UserData userData = UserManager.Instance.GetCurrentUser();
        if (userData == null)
        {
            return;
        }
        if (IsOn)
        {
            userData.FavoriteCardUID(cardData.UID);
        }
        else
        {
            userData.UnfavoriteCardUID(cardData.UID);
        }
        //Potentially save changes right away
        //Get current user path
        string userPath = GameObject.FindObjectOfType<CardFolderPasser>().UserPath;
        if(userPath != null)
        {
            UserManager.SaveUser(userData, userPath, true, false);
        }
        
    }
}
