using FMODUnity;
using UnityEngine;

public class RestSpotWithUpgrade: RestSpot
{
    [SerializeField] private PowerupBuyMenu powerupBuyMenu;
    [SerializeField] private EventReference eventReference;
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        powerupBuyMenu.Show();
    }
}
