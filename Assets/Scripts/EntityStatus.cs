using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    [SerializeField] float maxHealth;
    List<Modifier> damageModifiers;
    List<Modifier> healModifiers;
    float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damage)
    {
        foreach (Modifier m in damageModifiers)
        {
            damage = m.Apply(damage);
        }

        currentHealth -= damage;
    }
    
    public void Heal(float heal)
    {
        foreach (Modifier m in healModifiers)
        {
            heal = m.Apply(heal);
        }

        currentHealth += heal;
    }
}
