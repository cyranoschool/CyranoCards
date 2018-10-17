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

    private void Awake()
    {
        layout = GetComponent<VerticalLayoutGroup>();
    }

    // Use this for initialization
    void Start ()
    {
        PopulateLayers();
        //Make sure spacing is correct at the very beginning of the game
        LerpSpacing(100f);
    }
	
	// Update is called once per frame
	void Update () {
        LerpSpacing(Time.deltaTime);
	}

    void LerpSpacing(float delta)
    {
        //lerp spacing to fit in new cards on screen
        int childCount = transform.childCount;
        float moveDelta = delta * SpacingSpeed * CardHeight / childCount;
        float spacingTarget = CardHeight / (transform.childCount) - CardHeight;
        layout.spacing = Mathf.MoveTowards(layout.spacing, spacingTarget, moveDelta);
    }

    void PopulateLayers()
    {
        CardManager.UnloadAll();
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
        storyGroup.name = "StoryGroup";
        Transform sectionGroup = GameObject.Instantiate(LayoutGroupPrefab, transform).transform;
        sectionGroup.name = "SectionGroup";
        Transform LineGroup = GameObject.Instantiate(LayoutGroupPrefab, transform).transform;
        LineGroup.name = "LineGroup";

        Dictionary<string, GameObject> cardRefs = new Dictionary<string, GameObject>();
        //Create card gameObjects and add them to dictionary
        storyCards.ForEach(x => cardRefs.Add(x.UID, CreateCardInGroup(x, storyGroup)));
        sectionCards.ForEach(x => cardRefs.Add(x.UID, CreateCardInGroup(x, sectionGroup)));
        lineCards.ForEach(x => cardRefs.Add(x.UID, CreateCardInGroup(x, LineGroup)));

        //Add child references to each card through ParentCard for all except for line
        foreach(StoryData story in storyCards)
        {
            string UID = story.UID;
            GameObject go = cardRefs[UID];
            ParentCard parenter = go.AddComponent<ParentCard>();
            parenter.LineCards = story.SectionsUID.ConvertAll<GameObject>(s => cardRefs[s]);
            //Layer must count downward for each level
            parenter.LayoutLayer = (int)ParentCard.Layer.Story;
            parenter.HideTransform = transform;
            parenter.HideCards();
        }
        foreach (SectionData section in sectionCards)
        {
            string UID = section.UID;
            GameObject go = cardRefs[UID];
            ParentCard parenter = go.AddComponent<ParentCard>();
            //parenter.LineCards = section.LinesUID.ConvertAll<GameObject>(s => cardRefs[s]);
            for (int i = 0; i < section.LinesUID.Count; i++)
            {
                GameObject foundObject = null;
                if(cardRefs.TryGetValue(section.LinesUID[i], out foundObject))
                {
                    parenter.LineCards.Add(foundObject);
                }
                else
                {
                    Debug.LogError($"{section.To}\n{section.BrokenUpTo.Split()[i]}\n{section.PhoneticFrom.Split()[i]}");
                }

            }
            parenter.LayoutLayer = (int)ParentCard.Layer.Section;
            parenter.HideTransform = transform;
            parenter.HideCards();
        }

        //For LineData add trigger for game instead
        foreach (LineData line in lineCards)
        {
            string UID = line.UID;
            GameObject go = cardRefs[UID];
            ParentCard parenter = go.AddComponent<ParentCard>();
            
            parenter.LayoutLayer = (int)ParentCard.Layer.Line;
            parenter.HideTransform = transform;
            parenter.HideCards();

            go.AddComponent<GameLineTrigger>();
        }

    }

    GameObject CreateCardInGroup(CardData cardData, Transform group)
    {
        GameObject go = GameObject.Instantiate(CardPrefab, group);
        go.GetComponent<LargeCard>().SetCard(cardData);
        return go;
    }
}