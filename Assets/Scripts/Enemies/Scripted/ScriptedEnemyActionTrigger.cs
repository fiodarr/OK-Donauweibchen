using System;
using UnityEngine;

public class ScriptedEnemyActionTrigger: MonoBehaviour
{
    [SerializeField] private bool IsOneTime = true;
    public event Action OnTriggerActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerController player)) return;
        
        OnTriggerActivated?.Invoke();
        if (IsOneTime)
        {
            Destroy(this.gameObject);
        }
    }
}
