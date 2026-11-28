using System;
using Scene.ScriptedObjects;
using UnityEngine;

public class FallingShip: TriggerableObject
{
    [SerializeField] private Animator anim;
    [SerializeField] private RestSpot newRestSpot;
    
    private bool used = false;

    public bool StartedFalling;
    public override void Trigger(Collider2D other)
    {
        if (!used)
        {
            Debug.Log("FallingShip");
            used = true;
            anim.SetTrigger("StartedFall");
            RestSpotManager.Instance.SetRestSpot(newRestSpot);
        }
    }
}
