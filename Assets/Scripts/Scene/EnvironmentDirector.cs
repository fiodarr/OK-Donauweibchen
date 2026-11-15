using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class EnvironmentDirector: MonoBehaviour
{
    //What affects intensity:
    //If player is in combat
    //How many enemies are around in combat
    //How dangerous enemies are
    [Header("Intensity Coefficient")]
    [SerializeField] private List<IntensityMonitor> monitors = new List<IntensityMonitor>();
    [SerializeField] private float intensityTransitionDuration = 3f;
    [SerializeField] private StudioEventEmitter fmodEvent;

    private float currentIntensityValue = 0f;

    public float CurrentIntensityValue => currentIntensityValue;
    private IntensityMonitor.IntensityLevel currentIntensityLevel; 

    private Coroutine intensityChangeCoroutine;
    

    public static EnvironmentDirector Instance = null;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (IntensityMonitor monitor in monitors)
        {
            monitor.OnIntensityChange += OnMonitorIntensityChange;
        }
        StartCoroutine(CheckForChanges());
    }

    private IntensityMonitor.IntensityLevel RecalculateIntensityLevel()
    {
        IntensityMonitor.IntensityLevel level = IntensityMonitor.IntensityLevel.None;
        
        foreach (IntensityMonitor monitor in monitors)
        {
            IntensityMonitor.IntensityLevel newLevel = monitor.Intensity;
            
            if (newLevel >  level)
            {
                level = newLevel;
            }
        }
        
        return level;
    }

    private void OnDestroy()
    {
        foreach (IntensityMonitor monitor in monitors)
        {
            monitor.OnIntensityChange -= OnMonitorIntensityChange;
        }
    }

    public static EnvironmentDirector GetInstance()
    {
        return Instance;
    }
    

    private void OnMonitorIntensityChange()
    {
        IntensityMonitor.IntensityLevel level = RecalculateIntensityLevel();
        if (currentIntensityLevel == level) return;
        currentIntensityLevel = level;
        
        if (intensityChangeCoroutine != null)
        {
            StopCoroutine(intensityChangeCoroutine);
        }

        float newTargetIntensityValue;

        switch (currentIntensityLevel)
        {
            case IntensityMonitor.IntensityLevel.None:
                newTargetIntensityValue = 0f;
                break;
            case IntensityMonitor.IntensityLevel.Low:
                newTargetIntensityValue = 0.33f;
                break;
            case IntensityMonitor.IntensityLevel.High:
                newTargetIntensityValue = 1f;
                break;
            default:
                newTargetIntensityValue = 0f;
                break;
        }
        
        intensityChangeCoroutine = StartCoroutine(ChangeIntensityLevel(newTargetIntensityValue));
    }

    private IEnumerator ChangeIntensityLevel(float newIntensity)
    {
        float intensityChangeSpeed = 1f / intensityTransitionDuration;

        while (!Mathf.Approximately(currentIntensityValue, newIntensity))
        {
            currentIntensityValue = Mathf.MoveTowards(
                currentIntensityValue,
                newIntensity,
                intensityChangeSpeed * Time.deltaTime
            );
            Debug.Log(currentIntensityValue);
            fmodEvent.SetParameter("Intensity", currentIntensityValue);
            yield return null;
        }

        currentIntensityValue = newIntensity;
        intensityChangeCoroutine = null;
    }
    
    private IEnumerator CheckForChanges()
    {
        while (true)
        {
            OnMonitorIntensityChange();
            yield return new WaitForSeconds(1f);
        }
    }
}
