using UnityEngine;

public class CollisionPopupTrigger: PopupTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        ShowPopup();
    }
}
