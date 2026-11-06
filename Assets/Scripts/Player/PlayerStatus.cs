using UnityEngine;

public class PlayerStatus : EntityStatus
{
    public bool movementLocked;
    private PlayerComponents playerComponents;

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
        movementLocked = true;
        playerComponents.lantern.FadeOut();
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        RestSpotManager.Instance.RespawnPlayer();
    }
}
