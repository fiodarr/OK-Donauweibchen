using UnityEngine;

public class StaticIntensityEmitter: MonoBehaviour, IIntensityEmitter
{
    [SerializeField] private float intensity;
    
    public float GetIntensity()
    {
        return intensity;
    }
}
