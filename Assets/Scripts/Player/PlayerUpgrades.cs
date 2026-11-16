using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades: MonoBehaviour
{
    private List<PlayerUpgrade> upgrades;
    [SerializeField] PlayerComponents playerComponents;

    public void AttachUpgrade(PlayerUpgrade upgrade)
    {
        if (upgrade==null)
        return;

        upgrades.Add(upgrade);
        upgrade.Apply(playerComponents);
    }

    public void DetachUpgrade(PlayerUpgrade upgrade)
    {
        if (upgrade==null)
        return;

        upgrades.Remove(upgrade);
        upgrade.Apply(playerComponents);
    }
}
