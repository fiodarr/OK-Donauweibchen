using UnityEngine;

public class ArenaStaticIntensityEmitter: StaticIntensityEmitter
{
    [SerializeField] private GameObject[] connectedObjects;
    public override IntensityMonitor.IntensityLevel GetIntensity()
    {
        bool hasSomebodyConnected = false;
        for (int i = 0; i < connectedObjects.Length; i++)
        {
            if (connectedObjects[i].activeInHierarchy)
            {
                hasSomebodyConnected = true;
                break;
            }
        }
        if (!hasSomebodyConnected) return IntensityMonitor.IntensityLevel.None;
        
        return base.GetIntensity();
    }
    
    public void Update()
    {
        Debug.Log(gameObject.name + " : " + GetIntensity());
    }
}
