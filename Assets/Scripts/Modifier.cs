using UnityEngine;

public abstract class Modifier : ScriptableObject
{
    public abstract DamageData Apply(DamageData damage);
    public abstract float Apply(float heal);
}
