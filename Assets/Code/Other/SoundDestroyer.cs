using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundDestroyer : MonoBehaviour
{
    public bool activated = false;
    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (source.isPlaying)
        {
            activated = true;
        }
        else if (activated)
        {
            Destroy(gameObject);
        }
    }
}
