using System.Collections;
using UnityEngine;

public class BassScripted: Bass
{
    [SerializeField] private ScriptedEnemyActionTrigger trigger;
    [SerializeField] private Transform scriptedChargePoint;

    private void Start()
    {
        trigger.OnTriggerActivated += ScriptedCharge;
        Stun();
    }

    private void ScriptedCharge()
    {
        Debug.Log("Charging");

        Unstun();

        StartCoroutine(ScriptedChargeRoutine());
    }
    
    private IEnumerator ScriptedChargeRoutine()
    {
        yield return Plunge(scriptedChargePoint.position);

        yield return RecoverFromCharge();

        hitSolid = false;
    }
    
}
