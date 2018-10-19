using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToSceneButton : MonoBehaviour {

    public string SceneName = "";

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(GoBackToScene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GoBackToScene()
    {
        //Card folder passer should still exist and take over properly
        SceneManager.LoadScene(SceneName);
    }
}
