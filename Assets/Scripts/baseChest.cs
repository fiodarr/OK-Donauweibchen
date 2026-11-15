using System.Collections.Generic;
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
            Vector2 offset = new Vector3(Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f));
            Instantiate(item,(Vector2)transform.position + offset,Quaternion.identity);
        }
        animator.SetTrigger("open");
    }
}
