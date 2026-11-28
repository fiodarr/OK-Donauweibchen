using UnityEngine;

public class baseChest : MonoBehaviour, IHookable
{
    [SerializeField] GameObject[] loot;
    [SerializeField] Animator animator;

    [SerializeField] GameObject particles;

    bool isOpen;

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public bool AllowedToHook()
    {
        return !isOpen;
    }

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (!isOpen)
        {
            animator.SetTrigger("open");
            isOpen = true;
        }
    }

    public void SpawnLoot()
    {
        foreach (var item in loot)
        {
            Vector2 offset1 = new Vector2(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.3f)
            );

            Instantiate(item, (Vector2)transform.position + offset1, Quaternion.identity);

        }
        CameraShake.Instance.Shake(0.1f, 0.1f);
        GamepadVibration.Instance.Vibrate(0.1f, 0.1f);
        Instantiate(particles, transform.position,Quaternion.identity);
    }
}