using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawStoryParser
{

    public TextAsset storyText;
    public SerializationManager.SavePathType SavePath = SerializationManager.SavePathType.Streaming;
    public string folderPath = "/Testing";

    enum EditType { None, Story, Section, Line }
    enum ParseType { From, BrokenUpFrom, BrokenUpTo, To }

    // Use this for initialization
    void Start()
    {
        ParseData(storyText.text);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ParseData(string s)
    {

        EditType editType = EditType.None;
        ParseType parseType = ParseType.From;


        StoryData story = null;
        SectionData section = null;
        LineData line = null;

        CardData current = null; //Can be story, section, line, or card. All inherit from carddata
        Dictionary<string, CardData> generatedCards = new Dictionary<string, CardData>();
        List<CardData> generatedWordCards = new List<CardData>();

        foreach (string word in s.Trim().Split())
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }
            switch (word)
            {
                case "#story":
                    editType = EditType.Story;
                    story = new StoryData();
                    current = story;
                    break;
                case "#section":
                    editType = EditType.Section;
                    section = new SectionData();
                    story.SectionsUID.Add(section.UID);
                    current = section;
                    break;
                case "*":
                    //Trim all whitespace
                    current.From = current.From.TrimEnd();
                    current.To = current.To.TrimEnd();
                    current.PhoneticFrom = current.PhoneticFrom.TrimEnd();
                    current.BrokenUpTo = current.BrokenUpTo.TrimEnd();

                    //Finish parsing line
                    if (editType == EditType.Section)
                    {
                        editType = EditType.Line;
                    }
                    //Create individual word cards for all stories/sections/lines
                    current.DataFinalize();
                    List<CardData> lineCards = current.GenerateWordCards();
                    lineCards.ForEach(x => {generatedWordCards.Add(x); generatedCards.Add(x.UID, x); });
                    current.AddCardReferences(lineCards);

                    parseType = ParseType.From;
                    generatedCards.Add(current.UID, current);
                    current = null;
                    break;
                case "$":
                    parseType = ParseType.BrokenUpFrom;
                    break;
                case "/":
                    parseType = ParseType.BrokenUpTo;
                    break;
                case "=":
                    parseType = ParseType.To;
                    break;
                default:
                    //If there is no defined current card this is a new line
                    if (current == null)
                    {
                        line = new LineData();
                        current = line;
                        section.LinesUID.Add(line.UID);
                    }
                    switch (parseType)
                    {
                        case ParseType.From:
                            current.From += word + " ";
                            break;
                        case ParseType.BrokenUpFrom:
                            current.PhoneticFrom += word + " ";
                            break;
                        case ParseType.BrokenUpTo:
                            current.BrokenUpTo += word + " ";
                            break;
                        case ParseType.To:
                            current.To += word + " ";
                            break;
                    }
                    break;
            }

        }
        string path = SerializationManager.CreatePath(folderPath, SavePath);
        //Finalize cards and repair their references to refer to cards that already exist

        //Save word cards before checking for references because CheckDefinitionRepair checks CardManager dictionary
        generatedWordCards.ForEach(x => x.DataFinalize());
        foreach (CardData c in generatedWordCards)
        {
            if (!CardManager.ContainsMatchingDefinition(c))
            {
                CardManager.SaveCard(c, path);
                CardManager.PlaceInDictionaries(c);
            }
        }
        //References fixed, add new cards to dictionary/save as well
        foreach (CardData c in generatedCards.Values)
        {
            //Don't resave this card, it is only in the dictionary to be used with CheckDefinitionRepair
            if(c.CardType == "CardData")
            {
                continue;
            }

            c.CheckDefinitionRepair(generatedCards);
            if (!CardManager.ContainsMatchingDefinition(c))
            {
                CardManager.SaveCard(c, path);
            }
        }

    }

}
