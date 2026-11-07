using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDirector: MonoBehaviour
{
    //What affects intensity:
    //If player is in combat
    //How many enemies are around in combat
    //How dangerous enemies are
    [Header("Intensity Coefficient")]
    [SerializeField] private List<IntensityMonitor> monitors = new List<IntensityMonitor>();
    [SerializeField] private float maxIntensity;
    [SerializeField] private float intensity;
    
    
    public float IntensityCoefficient
    {
        get
        {
            float intensitySum = 0;

            foreach (var monitor in monitors)
            {
                intensitySum += monitor.Intensity;
            }
            
            //TODO: make it sum up to some number between [0 and 1]

            if (intensitySum > maxIntensity) intensity = maxIntensity;
            
            
            float normalizedIntensity = intensitySum / maxIntensity;
            float delta = normalizedIntensity - 0.5f;

            return 0.5f + 2f * delta * delta * delta;
        }
    }
}
