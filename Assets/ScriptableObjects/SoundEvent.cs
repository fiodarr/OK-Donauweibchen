using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[CreateAssetMenu(fileName = "NewSoundEvent", menuName = "Audio/Sound Event")]
public class SoundEvent : ScriptableObject
{
    public String nameIndex;
    private EventInstance soundInstance;

    public void Play()
    {
        soundInstance = RuntimeManager.CreateInstance(nameIndex);
        soundInstance.start();
    }

    public void End()
    {
        soundInstance.release();
    }
    
    public void Stop()
    {
        soundInstance.stop(STOP_MODE.ALLOWFADEOUT);
        soundInstance.release();
    }
    
    public void Pause()
    {
        soundInstance.setPaused(true);
    }
    
    public void Resume()
    {
        soundInstance.setPaused(false);
    }

    public void SetParameter(string parameterName, float value)
    {
        soundInstance.setParameterByName(parameterName, value);
    }
    
    public void SetParameter(string parameterName, int value)
    {
        soundInstance.setParameterByName(parameterName, value);
    }
    
    public void PlayOnGameObject(GameObject gameObject)
    {
        soundInstance = RuntimeManager.CreateInstance(nameIndex);
        RuntimeManager.AttachInstanceToGameObject(soundInstance, gameObject);
        soundInstance.start();
    }
}
