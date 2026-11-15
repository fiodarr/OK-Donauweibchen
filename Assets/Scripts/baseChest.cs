using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class baseChest : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject[] loot;
    [SerializeField] Animator animator;
    public string GetInteractionPrompt()
    {
        return "Open";
    }

    public void Interact(GameObject interactor)
    {
        foreach (var item in loot)
        {
            Instantiate(item,transform.position,Quaternion.identity);
        }
        animator.SetTrigger("open");
    }
}
