using System;
using FMOD.Studio;
using UnityEngine;

public class ParameterChangeTrigger: MonoBehaviour
{
    [SerializeField] private string parameter;
    [SerializeField] private float value;
    [SerializeField] private bool isOneTime = true;
    
    private bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOneTime && isUsed)
            return;
        
        if (other.CompareTag("Player"))
        {
            EventInstance currentMusic = EnvironmentDirector.Instance.GetCurrentMusic();
            currentMusic.setParameterByName(parameter, value);
            isUsed = true;
        }
    }
}
