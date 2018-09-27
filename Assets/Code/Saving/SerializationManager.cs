using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;

public static class SerializationManager
{

    public static List<ISerializationSurrogate> Surrogates = new List<ISerializationSurrogate>();
    public static SurrogateSelector Selector = new SurrogateSelector();
    static SerializationManager()
    {
        Selector.AddSurrogate(typeof(Color32), new StreamingContext(StreamingContextStates.All), new Color32SerializationSurrogate());
    }


    /// <summary>
    /// Create a path to a file in the savedata folder
    /// </summary>
    /// <param name="filename">Name of the file to generate a path to</param>
    /// <returns>Full path to the file</returns>
    public static string CreatePath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, "SaveData/" + filename);
    }

    /// <summary>
    /// Create byte data for an object, allowing it to be saved for later usage
    /// </summary>
    /// <param name="graph">Object to be serialized</param>
    /// <returns>byte[] of data to be stored</returns>
    public static byte[] SerializeObject(object graph)
    {
        MemoryStream ms = new MemoryStream();
        SaveObject(ms, graph);
        ms.Flush();
        byte[] array = ms.ToArray();
        ms.Dispose();
        return array;
    }

    /// <summary>
    /// Serialize an object and save at the specified path
    /// </summary>
    /// <param name="stream">IO stream to write to</param>
    /// <param name="graph">Object to serialize</param>
    /// <returns>bool indicating whether the object saved</returns>
    public static bool SaveObject(Stream stream, object graph)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.SurrogateSelector = Selector;

        try
        {
            formatter.Serialize(stream, graph);
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
            return false;
        }
        return true;

    }

    /// <summary>
    /// Serialize an object and save at the specified path
    /// </summary>
    /// <param name="path">Path to save to</param>
    /// <param name="graph">Object to serialize</param>
    /// <returns>bool indicating whether the object saved</returns>
    public static bool SaveObject(string path, object graph)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string folderPath = Path.GetDirectoryName(path);
        try
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
            return false;
        }
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.SurrogateSelector = Selector;

            try
            {
                formatter.Serialize(stream, graph);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }
        Debug.LogFormat("Saved at: {0}", path);
        return true;
    }

    /// <summary>
    /// Load an object from a byte array
    /// </summary>
    /// <param name="array">Binary data to load</param>
    /// <returns>Returns the object loaded</returns>
    public static object LoadObject(byte[] array)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (MemoryStream stream = new MemoryStream(array))
        {

            formatter.SurrogateSelector = Selector;
            try
            {
                return formatter.Deserialize(stream);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Load an object at the specified path
    /// </summary>
    /// <param name="path">Full path to the object</param>
    /// <returns>Reference to the loaded  object</returns>
    public static object LoadObject(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogFormat("Could not load file: {0}\nFile does not exist!", path);
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            formatter.SurrogateSelector = Selector;
            try
            {
                Debug.LogFormat("Loading file: {0}", path);
                return formatter.Deserialize(stream);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Create a folder at the specified path
    /// </summary>
    /// <param name="folderPath">full path to the folder</param>
    /// <returns>bool indicating if the function succeeded</returns>
    public static bool CreateFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            Debug.LogFormat("Folder already exists: {0}", folderPath);
            return false;
        }
        try
        {
            Directory.CreateDirectory(folderPath);
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
            return false;
        }
        Debug.LogFormat("Created folder at: {0}", folderPath);
        return true;
    }

    /// <summary>
    /// Delete the file at the specifed path
    /// </summary>
    /// <param name="path">Full path to the file to be deleted</param>
    /// <returns>bool indicating if the function succeeded</returns>
    public static bool DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                Debug.LogFormat("Deleting file: {0}", path);
                File.Delete(path);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }
        else
        {
            Debug.LogFormat("Could not delete file: {0}\nFile does not exist!", path);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Delete a folder and all recursive folders if applicable
    /// </summary>
    /// <param name="path">Full path to the folder</param>
    /// <param name="recursive">Whether folders should be deleted recursively or not</param>
    /// <returns></returns>
    public static bool DeleteFolder(string path, bool recursive)
    {
        if (Directory.Exists(path))
        {
            try
            {
                Debug.LogFormat("Deleting folder: {0}", path);
                Directory.Delete(path, recursive);
            }
            catch (IOException ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }
        else
        {
            Debug.LogFormat("Could not delete folder: {0}\nFolder does not exist!", path);
            return false;
        }
        return true;
    }

    public static void SaveJsonObject(string path, object graph)
    {
        string json = JsonUtility.ToJson(graph);
        File.WriteAllText(path, json);
    }

    public static T LoadJsonObject<T>(string path)
    {

        if (!File.Exists(path))
        {
            Debug.LogFormat("Could not load file: {0}\nFile does not exist!", path);
            return default(T);
        }

        string dataAsJson = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(dataAsJson);
    }
}
