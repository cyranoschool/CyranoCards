using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class LargeCardEditor : MonoBehaviour
{

    [Header("Init")]
    public TMP_InputField FromField;
    public TMP_InputField PhoneticField;
    public TMP_InputField BrokenUpToField;
    public TMP_InputField ToField;

    LargeCard largeCard;
    CardData dupCardData;

    // Use this for initialization
    private void Start()
    {
        //There was problem with SelcorToLargeCard being placed after this monobehaviour every time the scene was saved
        //This script has to start after
        Invoke("Setup", 0);
    }
    void Setup()
    {
        //Create duplicate cardData to work with and apply to card
        largeCard = GetComponent<LargeCard>();
        dupCardData = largeCard.GetCardData().Duplicate();
        largeCard.SetCard(dupCardData);

        //Set individual field listeners
        FromField.onEndEdit.AddListener(SetFrom);
        PhoneticField.onEndEdit.AddListener(SetPhonetic);
        BrokenUpToField.onEndEdit.AddListener(SetBrokenUp);
        ToField.onEndEdit.AddListener(SetTo);

        //Set individual field pretext
        FromField.text = dupCardData.From;
        PhoneticField.text = dupCardData.PhoneticFrom;
        BrokenUpToField.text = dupCardData.BrokenUpTo;
        ToField.text = dupCardData.To;


        List<TMP_InputField> fields = new List<TMP_InputField>() { FromField, PhoneticField, BrokenUpToField, ToField };

        UnityAction<string> finalizeAndUpdate = x =>
        {
            dupCardData.DataFinalize();
            largeCard.UpdateCard();
        };

        foreach (TMP_InputField field in fields)
        {
            field.onEndEdit.AddListener(finalizeAndUpdate);
        }
    }

    public void SetFrom(string from)
    {
        dupCardData.From = from;
    }
    public void SetPhonetic(string phoneticFrom)
    {
        dupCardData.PhoneticFrom = phoneticFrom;
    }
    public void SetBrokenUp(string brokenUpTo)
    {
        dupCardData.BrokenUpTo = brokenUpTo;
    }
    public void SetTo(string to)
    {
        dupCardData.To = to;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
