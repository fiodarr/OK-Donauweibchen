using System.Collections.Generic;
using UnityEngine;

public class baseChest : MonoBehaviour, IHookable
{
    [SerializeField] GameObject[] loot;
    [SerializeField] Animator animator;

    bool isOpen;
    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (!isOpen)
        {
            foreach (var item in loot)
            {
                Vector2 offset1 = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                Instantiate(item, (Vector2)transform.position + offset1, Quaternion.identity);
            }
            animator.SetTrigger("open");
            isOpen = true;

        }
    }

   }
