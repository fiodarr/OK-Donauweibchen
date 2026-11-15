using UnityEngine;

public class RestSpotWithUpgrade: RestSpot
{
    [SerializeField] private PowerupBuyMenu powerupBuyMenu;
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        powerupBuyMenu.Show();
        Debug.Log("HEREHERE");
    }
}
