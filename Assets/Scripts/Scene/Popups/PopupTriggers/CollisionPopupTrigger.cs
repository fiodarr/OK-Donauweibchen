using UnityEngine;

public class CollisionPopupTrigger: PopupTrigger
{
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        ShowPopup();
    }
}
