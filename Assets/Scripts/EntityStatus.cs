using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    [SerializeField] float maxHealth;
    private List<Modifier> damageModifiers = new List<Modifier>();
    private List<Modifier> healModifiers = new List<Modifier>();
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
        if (damageModifiers.Count != 0)
        {
            foreach (Modifier m in damageModifiers)
            {
                damage = m.Apply(damage);
            }

            if (damage <= 0)
                return;
        }
        currentHealth -= damage;
    }
    
    public void Heal(float heal)
    {
        if (healModifiers.Count != 0)
        {
            foreach (Modifier m in healModifiers)
            {
                heal = m.Apply(heal);
            }

            if (heal <= 0)
                return;
        }
        currentHealth += heal;
    }
    public void AddDamageModifier(Modifier modifier)
    {
        damageModifiers.Add(modifier);
    }
    public void AddHealModifier(Modifier modifier)
    {
        healModifiers.Add(modifier);
    }
}
