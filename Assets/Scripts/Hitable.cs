
using UnityEngine;

public abstract class Hitable : MonoBehaviour
{
    public abstract void TakeDamage(Vector2 damageDirection, float damage);
}
