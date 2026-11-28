using System.Collections.Generic;
using UnityEngine;

public class RestSpotManager : MonoBehaviour
{
    public static RestSpotManager Instance;

    public PlayerComponents playerComponents;
    public Transform player;
    public List<BaseEnemy> enemies;
    [SerializeField] private RestSpot lastRestSpot;
    private bool isResting;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateRestSpot(RestSpot restSpot)
    {
        restSpot.Activate();

        lastRestSpot = restSpot;

        PlayerPrefs.SetString("LastRestSpot", restSpot.restSpotID);
        PlayerPrefs.Save();
    }

    // Called when the player interacts with a RestSpot.
    public void RestAtRestSpot(RestSpot restSpot)
    {
        if (isResting)
        {
            FinishResting();
            RespawnEnemies();
        }
        else
        {

            isResting = true;
            playerComponents.lantern.Refill();
            ActivateRestSpot(restSpot);
            playerComponents.status.Rest();
        }
    }

    // Called when the player finishes resting and stands up.
    public void FinishResting()
    {
        isResting = false;
        playerComponents.status.FinishRest();
    }


    private void RespawnEnemies()
    {
        foreach (BaseEnemy enemy in enemies)
        {
            enemy.Respawn();
            Debug.Log("res");
        }
        
    }

    public void RespawnPlayer()
    {
        if (lastRestSpot == null) return;

        playerComponents.transform.position =
            lastRestSpot.transform.position + Vector3.up;

        playerComponents.status.Rest();
        playerComponents.status.FinishRest();
        playerComponents.lantern.RestoreLight();
        RespawnEnemies();
    }

    public void SetRestSpot(RestSpot restSpot)
    {
        lastRestSpot = restSpot;
    }
}