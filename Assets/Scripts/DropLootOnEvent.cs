using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropLootOnEvent : MonoBehaviour
{
    [SerializeField] private List<LootData> lootData = new List<LootData>();

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float randomOffset = 0.5f;

    public Transform SpawnPoint => spawnPoint;
    public float RandomOffset => randomOffset;

    private HashSet<int> dropped = new HashSet<int>();

    private void Awake()
    {
        if (!spawnPoint)
        {
            spawnPoint = transform;
        }
    }

    public void SpawnLoot()
    {
        for (int i = 0; i < lootData.Count; i++)
        {
            LootData data = lootData[i];

            if (dropped.Contains(i) && !data.Reused)
                continue;

            if (SpawnPoint == null || data.Loot.Count == 0)
                continue;

            dropped.Add(i);

            for (int j = 0; j < data.Quantity; j++)
            {
                GameObject lootObject = data.Loot[
                    Random.Range(0, data.Loot.Count)
                ];

                if (lootObject == null)
                    continue;

                Vector3 spawnPosition = SpawnPoint.position;

                spawnPosition.x += Random.Range(
                    -RandomOffset,
                    RandomOffset
                );

                spawnPosition.y += Random.Range(
                    -RandomOffset,
                    RandomOffset
                );

                Instantiate(lootObject, spawnPosition, Quaternion.identity);
            }
        }
    }
}