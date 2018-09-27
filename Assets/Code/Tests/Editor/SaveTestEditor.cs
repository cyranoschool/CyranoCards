using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveTest))]
public class SaveTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("Need to be in Play mode to test!", EditorStyles.helpBox);
        }
            DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            GUILayout.Space(24);
            GUILayout.Label("Play to do test.", EditorStyles.boldLabel);
            return;
        }

        SaveTest test = (SaveTest)target;
        if (GUILayout.Button("Save"))
        {
            test.SaveData();
        }
        if(GUILayout.Button("Clear Data"))
        {
            test.ClearData();
        }
        if (GUILayout.Button("Load"))
        {
            test.LoadData();
        }
        if(GUILayout.Button("Delete File"))
        {
            test.DeleteFile();
        }
    }
}