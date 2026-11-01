using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float maxArmor;
    private List<Modifier> damageModifiers = new List<Modifier>();
    private List<Modifier> healModifiers = new List<Modifier>();
    [SerializeField] float armorReduction;
    float currentHealth;
    float currentArmor;
    
    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }
    
    public float GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(DamageData[] damages)
    {
        foreach (DamageData originalDamage in damages)
        {
            DamageData damage = originalDamage;

            foreach (Modifier m in damageModifiers)
            {
                damage = m.Apply(damage);
            }

            damage.amount = Mathf.Max(0, damage.amount);

            switch (damage.type)
            {
                case DamageType.Armor:
                    currentArmor = Mathf.Max(0, currentArmor - damage.amount);
                    break;

                case DamageType.Physical:
                    if (currentArmor > 0)
                    {
                        damage.amount -= damage.amount*armorReduction;
                    }
                    currentHealth = Mathf.Max(0, currentHealth - damage.amount);
                    break;
            }
        }
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
