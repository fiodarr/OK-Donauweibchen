using UnityEngine;

public class ConditionalRodCollisionPopupTrigger: PopupTrigger
{
    [SerializeField] private PlayerCombat playerCombat;
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (playerCombat.hasFishingRod)
            ShowPopup();
    }
}
