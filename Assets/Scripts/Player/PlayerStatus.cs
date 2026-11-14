using System;
using FMODUnity;
using UnityEngine;

public class PlayerStatus : EntityStatus
{
    public bool movementLocked;
    private PlayerComponents playerComponents;
    [SerializeField] private EventReference dieSound;
    private FMOD.Studio.EventInstance dieSnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Death");


    private void Awake()
    {
        playerComponents = GetComponent<PlayerComponents>();
    }

    private void OnEnable()
    {
        OnDeath += Die;
    }

    private void OnDisable()
    {
        OnDeath -= Die;
    }
    protected override void FlashOnTakeDamage()
    {
        if (flashController != null)
        {

        }
    }

    public void Rest()
    {
        ResetStatus();
        movementLocked = true;
    }

    public void FinishRest()
    {
        movementLocked = false;
    }

    public void Die()
    {
        RuntimeManager.PlayOneShot(dieSound);
        dieSnapshot.start();
        movementLocked = true;
        playerComponents.lantern.FadeOut();
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        dieSnapshot.release();
        RestSpotManager.Instance.RespawnPlayer();
    }
}
