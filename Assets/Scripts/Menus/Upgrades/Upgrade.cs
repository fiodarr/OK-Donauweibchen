using UnityEngine;

public abstract class Upgrade: ScriptableObject
{
    public Sprite upgradeSprite;
    public string upgradeName;
    public string upgradeDescription;
    
    public abstract void Apply(PlayerComponents player);
}
