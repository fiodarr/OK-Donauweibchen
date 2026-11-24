using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootData
{
    [SerializeField] private List<GameObject> loot = new List<GameObject>();
    [SerializeField] private bool reused;
    
    [SerializeField] private int quantity = 1;

    public List<GameObject> Loot => loot;
    public bool Reused => reused;
   
    public int Quantity => quantity;
}