using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawStoryParser : MonoBehaviour
{

    public TextAsset storyText;

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
        CardData card = null;

        CardData current = null; //Can be story, section, line, or card. All inherit from carddata

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
                    story.Sections.Add(section);
                    current = section;
                    break;
                case "*":
                    //Finish parsing line
                    if (editType == EditType.Section)
                    {
                        editType = EditType.Line;
                    }
                    //Trim all whitespace
                    current.From = current.From.TrimEnd();
                    current.To = current.To.TrimEnd();
                    current.BrokenUpFrom = current.BrokenUpFrom.TrimEnd();
                    current.BrokenUpTo = current.BrokenUpTo.TrimEnd();

                    parseType = ParseType.From;
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
                        section.Lines.Add(line);
                    }
                    switch (parseType)
                    {
                        case ParseType.From:
                            current.From += word + " ";
                            break;
                        case ParseType.BrokenUpFrom:
                            current.BrokenUpFrom += word + " ";
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

        string path = SerializationManager.CreatePath("test.json", SerializationManager.SavePathType.TempCache);
        SerializationManager.SaveJsonObject(path, story, true);
    }

}
