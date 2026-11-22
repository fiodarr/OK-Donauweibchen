using FMODUnity;
using UnityEngine;
using EventReference = Mono.Cecil.EventReference;

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
