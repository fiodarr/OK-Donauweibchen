using System;
using UnityEngine;

public class ScriptedEnemyActionTrigger: MonoBehaviour
{
    [SerializeField] private bool IsOneTime = true;
    public event Action OnTriggerActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name + " tried to enter here");
        if (!other.gameObject.TryGetComponent(out PlayerController player)) return;
        Debug.Log("player entered trigger");
        OnTriggerActivated?.Invoke();
        if (IsOneTime)
        {
            Destroy(this.gameObject);
        }
    }
}
