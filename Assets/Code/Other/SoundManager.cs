using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    static SoundManager instance;
    Dictionary<string, AudioClip> resourceSounds = new Dictionary<string, AudioClip>();

    void Awake()
    {
        instance = this;
        foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>(""))
        {
            resourceSounds.Add(audioClip.name, audioClip);
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static AudioSource GetSound(string name, bool autoKill = true, Transform parent = null, float pitchShiftRange = 0)
    {
        if(!instance.resourceSounds.ContainsKey(name))
        {
            Debug.LogError($"Sound \"{name}\" does not exist!");
            return null;
        }
        return GetSound(instance.resourceSounds[name], autoKill, parent, pitchShiftRange);
    }

    public static AudioSource GetSound(AudioClip clip, bool autoKill = true, Transform parent = null, float pitchShiftRange = 0)
    {
        if (parent == null)
        {
            parent = instance.transform;
        }
        GameObject sound = new GameObject();
        sound.transform.SetParent(parent);
        AudioSource source = sound.AddComponent<AudioSource>();
        source.clip = clip;
        source.pitch = Random.Range(1f - pitchShiftRange, 1f + pitchShiftRange);
        if (autoKill)
        {
            sound.AddComponent<SoundDestroyer>();
        }
        sound.name = clip.name;
        return source;
    }
}