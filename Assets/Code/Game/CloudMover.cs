using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour {

    [Header("Config")]
    public float range = 100f;
    public float speed = .5f; //speed in units

    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        float timeScale = speed / range;
        float xOffset = range * Mathf.Sin(Time.timeSinceLevelLoad * timeScale);
        transform.position = Vector3.right * xOffset + startPos;
	}
}
