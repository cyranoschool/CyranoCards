using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UserMenuGenerator : MonoBehaviour
{

    [Header("Init")]
    public GameObject UserPrefab;
    public GameObject LanguagePrefab;

    public Transform UserLayout;
    public Transform LanguageLayout;

    [Header("Config")]
    public SerializationManager.SavePathType PathType = SerializationManager.SavePathType.Streaming;
    public string UserFolder = "Users";

    // Use this for initialization
    void Start()
    {
        PopulateUsers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PopulateUsers()
    {
        string usersPath = SerializationManager.CreatePath(UserFolder + "/", PathType);
        string[] directories = Directory.GetDirectories(usersPath);

        for (int i = 0; i < directories.Length; i++)
        {
            string folderPath = directories[i];
            string userDataText = File.ReadAllText(folderPath + "/user.json");
            UserData userData = JsonUtility.FromJson<UserData>(userDataText);


            GameObject go = GameObject.Instantiate(UserPrefab, UserLayout);
            go.transform.position = Vector3.zero;
            CanvasGroup cg = go.GetComponent<CanvasGroup>();
            cg.interactable = true;
            cg.blocksRaycasts = true;
            go.GetComponentInChildren<TextMeshProUGUI>().text = userData.Name;

            //Add trigger
            //Name is set to the folderpath
            go.name = folderPath + "/Languages";
            EventTrigger trigger = go.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { TappedUser((PointerEventData)data); });
            trigger.triggers.Add(entry);

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }



    void PopulateLanguages(string directory)
    {
        string[] directories = Directory.GetDirectories(directory);

        for (int i = 0; i < directories.Length; i++)
        {
            string folderPath = directories[i];

            GameObject go = GameObject.Instantiate(LanguagePrefab, LanguageLayout);
            go.transform.position = Vector3.zero;
            CanvasGroup cg = go.GetComponent<CanvasGroup>();
            cg.interactable = true;
            cg.blocksRaycasts = true;
            go.GetComponentInChildren<TextMeshProUGUI>().text = new DirectoryInfo(folderPath).Name;

            //Add trigger
            go.name = folderPath;
            EventTrigger trigger = go.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { TappedLanguage((PointerEventData)data); });
            trigger.triggers.Add(entry);

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    void TappedUser(PointerEventData data)
    {
        GameObject go = data.pointerCurrentRaycast.gameObject;
        string folderPath = go.name;
        //Destroy all children
        for (int i = LanguageLayout.childCount - 1; i >= 0; i--)
        {
            Destroy(LanguageLayout.GetChild(i).gameObject);
        }
        //Repopulate languages
        PopulateLanguages(folderPath);
    }

    void TappedLanguage(PointerEventData data)
    {
        GameObject go = data.pointerCurrentRaycast.gameObject;
        string folderPath = go.name;
        DirectoryInfo dInfo = new DirectoryInfo(folderPath);

        //Don't rebuild if it already exists
        CardFolderPasser passer = GameObject.FindObjectOfType<CardFolderPasser>();
        if (passer == null)
        {
            GameObject po = new GameObject();
            passer = po.AddComponent<CardFolderPasser>();
        }
        //This is the full name path so make sure to switch from streaming path creation to no path
        passer.FolderPath = dInfo.FullName;
        //Always keep around to move back to the menu if needed
        passer.DestroyAfterLoad = false;
        passer.DestroyOnWrongLevel = false;
        passer.LevelTarget = "TreeMenu";
        SceneManager.LoadScene("TreeMenu");
    }

}

/// <summary>
/// After it loads into the menu scene it sets the proper folder
/// </summary>
public class CardFolderPasser : SceneDataPasser
{
    public string FolderPath = "";
    protected override void DoAfterLoad()
    {
        base.DoAfterLoad();
        MenuTreeGenerator generator = GameObject.FindObjectOfType<MenuTreeGenerator>();
        generator.StoryFolder = FolderPath;
        //This path is the full path
        generator.PathType = SerializationManager.SavePathType.FileNameOnly;
        Debug.Log("set");
    }
}