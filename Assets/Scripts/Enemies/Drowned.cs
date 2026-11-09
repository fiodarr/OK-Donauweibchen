using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Drowned : BaseEnemy
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float loseTargetRange = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private FlashController flashController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private SoundEvent step;
    [SerializeField] Transform detectionPivot;

    private Transform target;
    [SerializeField] private Animator animator;
    private Coroutine attackRoutine;
    private Vector2 direction;

    private bool canMove = true;
    private bool recoveryEnded = false;

    [SerializeField] private AttackData attack;

    private void Update()
    {
        if (currentState != EnemyState.Idle)
            return;

        if (target == null)
        {
            DetectPlayer();
            return;
        }

        if (Vector2.Distance(detectionPivot.position, target.position) > loseTargetRange)
            target = null;
    }

    private void FixedUpdate()
    {
        if (currentState != EnemyState.Idle)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        if (target != null &&
            Vector2.Distance(detectionPivot.position, target.position) < attackRange)
        {
            if (attackRoutine == null)
            {
                attackRoutine = StartCoroutine(PerformAttack());
            }

            return;
        }

        if (target == null)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        if (!canMove)
        {
            Rb.linearVelocity = Vector2.zero;
            return;
        }

        direction = ((Vector2)target.position - Rb.position).normalized;

        Rb.linearVelocity = direction * moveSpeed;

        animator.SetBool("IsMoving", true);
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    private IEnumerator PerformAttack()
    {
        Debug.Log("Attack start");
        recoveryEnded = false;
        SetState(EnemyState.Attack);
        animator.SetBool("IsInWindUp", true);

        yield return new WaitForSeconds(2f);

        animator.SetBool("IsInWindUp", false);
        animator.SetBool("IsStuck", true);
        SetState(EnemyState.Vulnerable);


        yield return new WaitForSeconds(3f);

        animator.SetBool("IsStuck", false);

        yield return new WaitUntil(() => recoveryEnded == true);

        currentState = EnemyState.Idle;

        yield return new WaitForSeconds(1f);

        attackRoutine = null;
    }
    //        flashController.FlashMultiple(5, 0.1f, 0.1f, 1f);
    private void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(
            detectionPivot.position,
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
        Vector3 pivotPosition = detectionPivot != null
            ? detectionPivot.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivotPosition, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivotPosition, loseTargetRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivotPosition, attackRange);
    }

    public void AllowMove()
    {
        canMove = true;
    }

    public void ActivateHitbox()
    {
        attack.hitbox.BeginAttack(
            direction,
            attack.damage,
            attack.knockback
        );
    }

    public void DeActivateHitbox()
    {
        attack.hitbox.EndAttack();
    }

    public void RecoveryEnd()
    {
        recoveryEnded = true;
    }

    public void ForbidMove()
    {
        canMove = false;
        RuntimeManager.PlayOneShot(step.nameIndex, transform.position);
    }

    public override void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (currentState!=EnemyState.Attack)
            StartCoroutine(HookRoutine(origin, direction, offset, pullSpeed));
    }


    protected override void OnPullStart()
    {
        animator.SetBool("IsHooked", true);
    }

    protected override void OnPullEnd()
    {
        animator.SetBool("IsHooked", false);
    }
}