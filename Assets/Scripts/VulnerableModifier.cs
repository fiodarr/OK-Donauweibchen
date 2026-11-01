using UnityEngine;

[CreateAssetMenu(menuName = "Modifiers/Vulnerable Modifier")]
public class VulnerableModifier : Modifier
{
    public override DamageData Apply(DamageData damage)
    {
        damage.amount*=2;
        return damage;
    }

    public override float Apply(float heal)
    {
        return heal;
    }
}
