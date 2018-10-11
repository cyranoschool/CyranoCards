using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuTreeGenerator : MonoBehaviour {

    [Header("Init")]
    public GameObject CardPrefab;
    public GameObject LayoutGroupPrefab;

    [Header("Config")]
    public float CardHeight = 560f;
    public float VerticalSpacing = 240f;
    public float SpacingSpeed = 1f;

    public string StoryFolder = "Testing";

    VerticalLayoutGroup layout;
	// Use this for initialization
	void Start () {
        layout = GetComponent<VerticalLayoutGroup>();

        //PopulateLayers();
    }
	
	// Update is called once per frame
	void Update () {
        //lerp spacing to fit in new cards on screen
        int childCount = transform.childCount;
        float moveDelta = Time.deltaTime * SpacingSpeed * CardHeight / childCount;
        float spacingTarget = CardHeight / (transform.childCount) - CardHeight;
        layout.spacing = Mathf.MoveTowards(layout.spacing, spacingTarget, moveDelta);
	}

    void PopulateLayers()
    {
        List<CardData> loadedCards = CardManager.LoadFolder(StoryFolder, true, SearchOption.AllDirectories);
        List<StoryData> storyCards = new List<StoryData>();
        List<SectionData> sectionCards = new List<SectionData>();
        List<LineData> lineCards = new List<LineData>();
        
        foreach(CardData card in loadedCards)
        {
            switch (card.CardType)
            {
                case "StoryData":
                    storyCards.Add((StoryData)card);
                    break;
                case "SectionData":
                    sectionCards.Add((SectionData)card);
                    break;
                case "LineData":
                    lineCards.Add((LineData)card);
                    break;
                default:
                    break;
            }
        }

        Transform storyGroup = GameObject.Instantiate(LayoutGroupPrefab, transform).transform;
        Transform sectionGroup = GameObject.Instantiate(LayoutGroupPrefab, transform).transform;
        Transform LineGroup = GameObject.Instantiate(LayoutGroupPrefab, transform).transform;

        foreach (StoryData story in storyCards)
        {
            //Create story card
            CreateCardInGroup(story, storyGroup);
            
        }


    }

    void CreateCardInGroup(CardData cardData, Transform group)
    {
        GameObject.Instantiate(CardPrefab, group);
    }
}