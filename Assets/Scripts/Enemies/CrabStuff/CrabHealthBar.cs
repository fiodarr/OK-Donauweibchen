using System;
using UnityEngine;

public class CrabHealthBar: MonoBehaviour
{
    [SerializeField] private Crab crab;
    [SerializeField] private HealthChunk[] healthChunks;
    private float oneChunkHp;
    private EntityStatus crabEntityStatus;
    
    private void Start()
    {
        crabEntityStatus = crab.EntityStatus;
        crabEntityStatus.OnHealthChanged += UpdateHealthBar;
        crab.OnArmorPhaseChange += UpdateArmorPhase;
        oneChunkHp = crab.EntityStatus.GetMaxHealth() / healthChunks.Length;
        SetupChunks(oneChunkHp);
        
        UpdateHealthBar(
            crabEntityStatus.GetHealth(),
            crabEntityStatus.GetMaxHealth()
        );
    }

    private void UpdateHealthBar(float newHp, float maxHp)
    {
        Debug.Log($"HP: {newHp}/{maxHp}");

        for (int i = 0; i < healthChunks.Length; i++)
        {
            float chunkHp = Mathf.Clamp(
                newHp - oneChunkHp * (healthChunks.Length - i - 1),
                0,
                oneChunkHp
            );
            
            SetNewHpToChunk(chunkHp, i);

            Debug.Log($"Chunk {i}: {chunkHp}/{oneChunkHp}");
        }
    }

    private void UpdateArmorPhase()
    {
        int armorPhase = crab.ArmorPhase;

        for (int i = 0; i < healthChunks.Length; i++)
        {
            if (i <= armorPhase)
                RemoveArmorOfChunk(i);
        }
    }
    
    public void SetupChunks(float chunkMaxValue)
    {
        foreach (HealthChunk chunk in healthChunks)
        {
            chunk.SetMaxValue(chunkMaxValue);
        }
    }
    
    public void SetNewHpToChunk(float newChunkHp, int chunk)
    {
        newChunkHp = Math.Clamp(
            newChunkHp,
            0,
            healthChunks[chunk].GetMaxValue()
        );

        healthChunks[chunk].SetCurrentValue(newChunkHp);
    }

    public void RemoveArmorOfChunk(int chunk)
    {
        healthChunks[chunk].RemoveArmor();
    }
    
    private void OnDestroy()
    {
        if (crabEntityStatus != null)
            crabEntityStatus.OnHealthChanged -= UpdateHealthBar;
        if (crab != null) 
            crab.OnArmorPhaseChange -= UpdateArmorPhase;
    }
}
