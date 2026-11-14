using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerStatus : EntityStatus
{
    public bool movementLocked;
    private PlayerComponents playerComponents;
    [SerializeField] private EventReference dieSnapshot;
    [SerializeField] private EventReference dieSound;
    [SerializeField] private EventReference dieBubbles;
    private EventInstance deathSnap;


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
        RuntimeManager.PlayOneShot(dieBubbles);
        deathSnap = RuntimeManager.CreateInstance(dieSnapshot);
        deathSnap.start();
        movementLocked = true;
        playerComponents.lantern.FadeOut();
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        deathSnap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        deathSnap.release();
        RestSpotManager.Instance.RespawnPlayer();
    }
}
