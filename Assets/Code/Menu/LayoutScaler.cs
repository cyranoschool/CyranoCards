using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Makes layoutgroup smaller to fit all cards on the screen when necessary
/// </summary>
public class LayoutScaler : MonoBehaviour {

    public float LayoutElementWidth = 400f;
    public float Spacing = 0;

    CanvasScaler canvasScaler;

	// Use this for initialization
	void Start () {
        canvasScaler = GetComponentInParent<CanvasScaler>();
	}
	
	// Update is called once per frame
	void Update () {
        //If there are too many children that won't fit, scale the recttransform down
        float childrenTotalWidth = transform.childCount * (LayoutElementWidth + Spacing);
        float desiredScale = canvasScaler.referenceResolution.x / childrenTotalWidth;
        float scale = Mathf.Min(1f, desiredScale);
        RectTransform rectT = (RectTransform)transform;
        rectT.localScale = scale * Vector3.one;

    }
}
