using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header ("Init")]
    public Transform Body;
    public Joystick joystick;

    [Header("Config")]
    public float Acceleration = 2f;

    [Header("Input Axes")] //For viewing in editor
    [SerializeField]
    float inX;
    [SerializeField]
    float inY;
    [SerializeField]
    Vector2 input;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Flip body of sprite depending on direction
        if(inX != 0)
        {
            Body.transform.rotation = Quaternion.Euler(0, (inX < 0)? 0 : 180f, 0);
        }
       
    }

    private void FixedUpdate()
    {
        //Consider using getaxisraw for less "floaty" input
        inX = Input.GetAxisRaw("Horizontal");
        inY = Input.GetAxisRaw("Vertical");
        if(joystick != null)
        {
            inX += joystick.Horizontal;
            inY += joystick.Vertical;
        }
        input = Vector3.right * inX + Vector3.up * inY;
        input.Normalize();

        //Cut off input if there isn't much of it
        //This is to stop faulty presses or 0 input scenarios
        float mag = Mathf.Abs(inX) + Mathf.Abs(inY);
        float cutoff = 1;
        if(mag < .02f)
        {
            cutoff = 0;
        }
        

        rb.AddForce(input * cutoff * Acceleration);
    }
}
