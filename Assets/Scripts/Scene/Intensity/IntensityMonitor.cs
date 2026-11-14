using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntensityMonitor: MonoBehaviour
{
    public enum IntensityLevel
    {
        None = 0,
        Low = 1,
        High = 2
    }
    
    private HashSet<IIntensityEmitter> emitters = new HashSet<IIntensityEmitter>();
    
    public IntensityLevel Intensity
    {
        get
        {
            IntensityLevel level = IntensityLevel.None;
            
            foreach (var emitter in emitters)
            {
                IntensityLevel newLevel = emitter.GetIntensity();
                if (newLevel > level)
                {
                    level = newLevel;
                }
                
            }
            
            return level;
        }
    }
    
    public Action OnIntensityChange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name + " entered to monitor");
        if (other.TryGetComponent(out IIntensityEmitter intensityEmitter))
        {
            emitters.Add(intensityEmitter);
            OnIntensityChange?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IIntensityEmitter intensityEmitter))
        {
            emitters.Remove(intensityEmitter);
            OnIntensityChange?.Invoke();
        }
    }
}
