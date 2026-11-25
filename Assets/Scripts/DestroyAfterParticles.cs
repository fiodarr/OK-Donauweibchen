using UnityEngine;

public class DestroyAfterParticles : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particles.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}