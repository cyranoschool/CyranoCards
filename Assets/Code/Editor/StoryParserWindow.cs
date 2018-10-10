using UnityEngine;
using UnityEditor;
using System.IO;

public class StoryParserWindow : EditorWindow
{
    string folderPath = "Testing/TestStory";
    string rawText = "Put raw story here";
    Vector2 scroll;
    SerializationManager.SavePathType pathType = SerializationManager.SavePathType.Streaming;

    [MenuItem("Window/Custom/StoryParser")]
    public static void ShowWindow()
    {
        GetWindow<StoryParserWindow>("Story Parser");
    }

    void OnGUI()
    {
        GUILayout.Label("Story Parser", EditorStyles.boldLabel);

        pathType = (SerializationManager.SavePathType)EditorGUILayout.EnumPopup("Save path:", pathType);
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        string path = SerializationManager.CreatePath(folderPath, pathType);
        GUILayout.Label($"Path:\n{path}");
        bool dirExists = Directory.Exists(path);
        if (!dirExists)
        {
            EditorGUILayout.HelpBox("Directory does not exist at path.", MessageType.Warning, true);
        }
        else
        {
            EditorGUILayout.HelpBox("Directory already exists at path!", MessageType.Warning, true);
        }
        
        GUILayout.Label("Story Raw Text", EditorStyles.boldLabel);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        rawText = EditorGUILayout.TextArea(rawText);
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Parse/Save Cards", new GUILayoutOption[] { GUILayout.Width(300), GUILayout.Height(32) }))
        {
            if (!EditorUtility.DisplayDialog("Parse text into current path?", "Cards will be added to path:\n" + path + "\nThis cannot be undone.", "Yes", "Cancel"))
            {
                return;
            }
            RawStoryParser parser = new RawStoryParser();
            parser.folderPath = folderPath;
            parser.SavePath = pathType;
            parser.ParseData(rawText);
        }
    }

}