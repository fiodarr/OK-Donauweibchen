using FMODUnity;
using System.Collections;
using UnityEngine;

public class Shrimp : BaseEnemy
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float loseTargetRange = 10f;
    [SerializeField] private float attackRange = 5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("References")]
    [SerializeField] private Animator animator;

    private Transform target;
    private Vector2 direction;
    private Coroutine attackRoutine;
    private bool canAttack = true;

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
        if (currentState != EnemyState.Idle)
            return;

        if (target == null)
            return;

        direction = ((Vector2)target.position - Rb.position).normalized;

        //animator.SetFloat("MoveX", direction.x);
        //animator.SetFloat("MoveY", direction.y);

        if (Vector2.Distance(transform.position, target.position) <= attackRange &&
            canAttack && attackRoutine == null)
        {
            attackRoutine = StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;
        SetState(EnemyState.Attack);

        //animator.SetBool("IsAttacking", true);


        Shoot();

        //animator.SetBool("IsAttacking", false);
        SetState(EnemyState.Idle);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        attackRoutine = null;
    }

    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(
            transform.position,
            detectionRange,
            Layers.Player
        );

        if (player != null)
            target = player.transform;
    }

    private void Shoot()
    {
        if (projectilePrefab == null || projectileSpawnPoint == null || target == null)
            return;

        Vector2 shootDirection =
            ((Vector2)target.position - (Vector2)projectileSpawnPoint.position).normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        projectile.GetComponent<PermanentHitbox>().AddToIgnore(this);
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        base.OnTakeDamage(damageDirection, damage, knockback);
    }

    protected override void Die()
    {
        StopAllCoroutines();
        base.Die();
       // animator.SetBool("IsDead", true);
    }

    public override void Respawn()
    {
        //animator.SetBool("IsDead", false);
        //animator.Rebind();
        //animator.Update(0f);
        canAttack = true;
        attackRoutine = null;
        target = null;
        base.Respawn();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pivotPosition = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivotPosition, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivotPosition, loseTargetRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivotPosition, attackRange);
    }
}