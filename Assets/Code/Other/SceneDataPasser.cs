using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for passing object data/functions to new scene
/// </summary>
public class SceneDataPasser : MonoBehaviour
{
    public bool DestroyAfterLoad = true;
    //Does this object wait around until the correct scene is loaded or destroy itself if loaded into wrong scene?
    public bool DestroyOnWrongLevel = true;
    //if null this will always fire no matter the level that was loaded (possibility of incorrect level)
    public string LevelTarget = null;

    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void DoAfterLoad()
    {

    }

    void OnLevelWasLoaded(int level)
    {
        //This scenedatapasser was not meant to be used on this scene
        if(LevelTarget != null && SceneManager.GetSceneByBuildIndex(level).name != LevelTarget)
        {
            if(DestroyOnWrongLevel)
            {
                Destroy(this);
            }
            return;
        }
        else
        {
            DoAfterLoad();
            if (DestroyAfterLoad)
            {
                Destroy(this);
            }
        }
    }
}
