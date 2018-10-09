using UnityEngine;
using UnityEditor;
using System.IO;

public class CardEditorWindow : EditorWindow
{
    string filename = "Testing/test.json";
    SerializationManager.SavePathType pathType = SerializationManager.SavePathType.Streaming;

    CardData cardData = new CardData();

    [MenuItem("Window/Custom/CardEditor")]
    public static void ShowWindow()
    {
        GetWindow<CardEditorWindow>("Card Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Card Editor", EditorStyles.boldLabel);
       
        pathType = (SerializationManager.SavePathType)EditorGUILayout.EnumPopup("Save path:", pathType);
        filename = EditorGUILayout.TextField("File Name", filename);

        string filePath = SerializationManager.CreatePath(filename, pathType);
        GUILayout.Label($"Path:\n{filePath}");
        bool fileExists = File.Exists(filePath);
        if (!fileExists)
        {
            EditorGUILayout.HelpBox("File does not exist at path!", MessageType.Error, true);
        }
        EditorGUI.BeginDisabledGroup(!fileExists);
        if (GUILayout.Button("Load Card"))
        {
            cardData = SerializationManager.LoadJsonObject<CardData>(filePath);
        }
        EditorGUI.EndDisabledGroup();
        if (fileExists)
        {
            EditorGUILayout.HelpBox("File already exists at path.\nAre you sure you want to overrite?", MessageType.Warning, true);
        }
        if (GUILayout.Button("Save Card"))
        {
            SerializationManager.SaveJsonObject(filePath, cardData, true);
        }

        //Card Data editing here
        GUILayout.FlexibleSpace();

        GUILayout.Label("CardData", EditorStyles.boldLabel);
        cardData.From = EditorGUILayout.TextField("From", cardData.From);
        cardData.To = EditorGUILayout.TextField("To", cardData.To);

        GUILayout.FlexibleSpace();
    }

}