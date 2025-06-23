using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixer audioMixer;
    void Awake()
    {
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
        }

    }

    public void Play(String name){
        //print("Play: " + name);
        Sound s = Array.Find(sounds ,sound => sound.name == name);
        s.source.Play();
    }
}
