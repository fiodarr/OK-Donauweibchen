using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public String nameIndex;
    public EventReference eventReference;


}
