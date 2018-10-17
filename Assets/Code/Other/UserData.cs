using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public string Name = "Unnamed";
    //Currently using GUID for unique ID
    //In a networked system the user id should just be the ID of the last user + 1
    public string UID = Guid.NewGuid().ToString();
}
