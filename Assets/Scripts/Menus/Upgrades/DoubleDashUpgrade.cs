public class DoubleDashUpgrade: Upgrade
{
    public override void Apply(PlayerComponents player)
    {
        player.combat.UpgradeDash();
    }
}
