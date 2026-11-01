using UnityEngine;

public abstract class Modifier : MonoBehaviour
{
    public abstract DamageData Apply(DamageData damage);
    public abstract float Apply(float heal);
}
