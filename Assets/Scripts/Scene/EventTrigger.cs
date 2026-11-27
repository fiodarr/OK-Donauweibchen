using Scene.ScriptedObjects;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger: MonoBehaviour
{
    [SerializeField] private TriggerableObject objectToTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectToTrigger.Trigger(collision);
    }
}
