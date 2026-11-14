using UnityEngine;

public class ArenaStaticIntensityEmitter: StaticIntensityEmitter
{
    [SerializeField] private BaseEnemy[] connectedObjects;
    public override IntensityMonitor.IntensityLevel GetIntensity()
    {
        bool hasSomebodyAlive = false;
        for (int i = 0; i < connectedObjects.Length; i++)
        {
            if (!connectedObjects[i].IsDead)
            {
                hasSomebodyAlive = true;
                break;
            }
        }
        if (!hasSomebodyAlive) return IntensityMonitor.IntensityLevel.None;
        
        return base.GetIntensity();
    }
}
