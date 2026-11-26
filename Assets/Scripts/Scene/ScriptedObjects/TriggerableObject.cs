using UnityEngine;

namespace Scene.ScriptedObjects
{
    public abstract class TriggerableObject: MonoBehaviour, ITriggerable
    {
        public abstract void Trigger(Collider2D other);
    }
}