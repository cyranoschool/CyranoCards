using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour {

    public static UserManager Instance;
    UserData currentUser;

    public Dictionary<string, UserData> usersUID = new Dictionary<string, UserData>();

    void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCurrentUser(UserData user)
    {
        currentUser = user;
    }
    public UserData GetCurrentUser()
    {
        return currentUser;
    }

    public static bool SaveUser(UserData userData, string path, bool prettyPrint = true, bool useDefaultName = true)
    {
        string name = "";
        if (useDefaultName)
        {
            name = "/" + userData.UID + ".json";
        }
        return SerializationManager.SaveJsonObject(path + name, userData, prettyPrint);
    }
    
}
