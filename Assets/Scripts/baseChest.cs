using System.Collections.Generic;
using UnityEngine;

public class baseChest : MonoBehaviour, IHookable
{
    [SerializeField] GameObject[] loot;
    [SerializeField] Animator animator;
    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        foreach (var item in loot)
        {
            Vector2 offset1 = new Vector3(Random.Range(-0.1f,0.1f),Random.Range(-0.1f,0.1f));
            Instantiate(item,(Vector2)transform.position + offset1,Quaternion.identity);
        }
        animator.SetTrigger("open");
    }

}
