using FMODUnity;
using UnityEngine;

public class Drowned : BaseEnemy
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float loseTargetRange = 10f;
    [SerializeField] private float attackRange = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private SoundEvent step;

    private Transform target;
    [SerializeField] private Animator animator;

    private bool canMove = true;

    private void Update()
    {
        if (currentState != EnemyState.Idle)
            return;

        if (target == null)
        {
            DetectPlayer();
            return;
        }

        if (Vector2.Distance(transform.position, target.position) > loseTargetRange)
            target = null;
    }

    private void FixedUpdate()
    {
        if (currentState != EnemyState.Idle ||
            target == null ||
            Vector2.Distance(transform.position, target.position) < attackRange ||
            !canMove)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        Vector2 direction = ((Vector2)target.position - Rb.position).normalized;

        Rb.linearVelocity = direction * moveSpeed;

        animator.SetBool("IsMoving", true);
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(
            Rb.position,
            detectionRange,
            Layers.Player
        );

        if (player != null)
            target = player.transform;
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        animator.SetTrigger("hit");
        base.OnTakeDamage(damageDirection, damage, knockback);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseTargetRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void AllowMove()
    {
        canMove = true;
    }

    public void ForbidMove()
    {
        canMove = false;
        RuntimeManager.PlayOneShot(step.nameIndex,transform.position);
    }
}