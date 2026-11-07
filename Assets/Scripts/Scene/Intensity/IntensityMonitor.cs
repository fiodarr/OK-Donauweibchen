using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntensityMonitor: MonoBehaviour
{
    private HashSet<IIntensityEmitter> emitters = new HashSet<IIntensityEmitter>();
    
    [SerializeField] protected float intensityModifier = 1f;
    
    public float Intensity
    {
        get
        {
            return emitters.Sum(emitter => emitter.GetIntensity()) * intensityModifier;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IIntensityEmitter intensityEmitter))
        {
            emitters.Add(intensityEmitter);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IIntensityEmitter intensityEmitter))
        {
            emitters.Remove(intensityEmitter);
        }
    }
}
