using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineManager : MonoBehaviour {

    [Header("Init Prefabs")]
    public GameObject CardDropOff;
    public GameObject CardPickup;
    //[Header("Init")]

    [Header("Config")]
    public string LineString;
    public string LanguageFolder;
    public bool reloadCards = false;

    List<List<string>> wordGroups;

    // Use this for initialization
    void Start () {
        
		if(reloadCards)
        {
            CardManager.LoadFolder(LanguageFolder);
        }
        BuildWords();
	}

    //WIP
    //Creates the various sets of words and parents
    void BuildWords()
    {
        string[] words = LineString.Split();

        //Each wordGroup is made up of cards with wordcount words.Length - i + 1
        wordGroups = new List<List<string>>();

        //Search for largest words first and add them to list then get smaller words
        StringBuilder sb = new StringBuilder();
        for(int wordSize = words.Length; wordSize > 0; wordSize--)
        {
            //The group of all words of the same length within this line
            List<string> currentGroup = new List<string>();
            wordGroups.Add(currentGroup);
            for (int i = 0; i <= words.Length - wordSize; i ++)
            {
                //Append all words together
                for(int j = 0; j < wordSize; j++)
                {
                    //Concatenate all words into single string to form a key
                    //e.g. "a" "b" "c" --> "a b c"
                    string word = words[i + j];
                    if (j > 0)
                    {
                        word = " " + word;
                    }
                    sb.Append(word);
                }
                string key = sb.ToString();
                sb.Clear();
                //Currently appending strings and not CardData
                var set = CardManager.GetCardsFrom(key);
                //Debug.Log(set.Count);
                currentGroup.AddRange(set.Select(x => x.From));
            }
        }
        
        foreach(var group in wordGroups)
        {
            foreach(string s in group)
            {
                sb.Append("\t" + s);
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
        
    }

    void CombineWords()
    {

    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
