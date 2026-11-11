using UnityEngine;

public class StaticIntensityEmitter: MonoBehaviour, IIntensityEmitter
{
    [SerializeField] private IntensityMonitor.IntensityLevel intensity;
    
    public IntensityMonitor.IntensityLevel GetIntensity()
    {
        return intensity;
    }
}
