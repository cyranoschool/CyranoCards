using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveTest : MonoBehaviour
{
    [Serializable]
    public class SaveTestData : ISerializationCallbackReceiver
    {
        public string text;
        public int num;

        public void OnBeforeSerialize()
        {
           // Debug.Log("OnBeforeSerialize");
        }

        public void OnAfterDeserialize()
        {
          // Debug.Log("OnAfterDeserialize");
        }
    }

    public SaveTestData data;
    string path;

    public void Awake()
    {
        path = SerializationManager.CreatePath("test" + ".tst");
    }

    public void ClearData()
    {
        data = new SaveTestData();
    }

    public void SaveData()
    {
        SerializationManager.SaveObject(path, data);
    }

    public void LoadData()
    {
        data = (SaveTestData)SerializationManager.LoadObject(path);
    }

    public void DeleteFile()
    {
        SerializationManager.DeleteFile(path);
    }

}
