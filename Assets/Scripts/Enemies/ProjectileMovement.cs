using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float speed = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector2 direction = transform.right;
        rb.linearVelocity = direction * speed;
    }
}