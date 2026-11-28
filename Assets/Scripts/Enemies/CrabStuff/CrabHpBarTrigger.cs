using UnityEngine;

public class CrabHpBarTrigger: MonoBehaviour
{
    [SerializeField] private CrabHealthBar crabHealthBar;
    [SerializeField] private RestSpot newRestSpot;
    [SerializeField] private GameObject blockingObject;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            crabHealthBar.gameObject.SetActive(true);
            RestSpotManager.Instance.SetRestSpot(newRestSpot);
            blockingObject.SetActive(true);
        }
    }
}
