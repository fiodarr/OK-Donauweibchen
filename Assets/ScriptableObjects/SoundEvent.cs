using System;
using FMODUnity;
using UnityEngine;
[CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public String nameIndex;
    
    public void Play()
    {
        RuntimeManager.PlayOneShot(nameIndex);
    }
}
