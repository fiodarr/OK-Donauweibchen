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
    private bool hasAttackSlot = false;

    private bool isStuck = false;

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
            hasAttackSlot = EnemyAttackManager.TryAcquire(this);
            if (!hasAttackSlot)
            {
                hasAttackSlot = EnemyAttackManager.TryAcquire(this);
            }

            if (hasAttackSlot && attackRoutine == null)
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



        direction = ((Vector2)target.position - Rb.position).normalized;

        if (!canMove)
        {
            Rb.linearVelocity = direction * moveSpeed;
        }

        animator.SetBool("IsMoving", true);
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    private IEnumerator PerformAttack()
    {
        var windUpInstance = RuntimeManager.CreateInstance(attack.chargeSound.nameIndex);
        RuntimeManager.AttachInstanceToGameObject(windUpInstance, gameObject);
        windUpInstance.start();
        Debug.Log("Attack start");
        recoveryEnded = false;
        SetState(EnemyState.Attack);
        animator.SetBool("IsInWindUp", true);

        yield return new WaitForSeconds(1f);

        animator.SetBool("IsInWindUp", false);
        windUpInstance.release();
        var attackInstance = RuntimeManager.CreateInstance(attack.attackSound.nameIndex);
        RuntimeManager.AttachInstanceToGameObject(attackInstance, gameObject);
        attackInstance.start();
        yield return new WaitUntil(() => isStuck == true);
        animator.SetBool("IsStuck", true);
        SetState(EnemyState.Vulnerable);
        PopupManager.Instance.TriggerPopup(PopupEvent.EnemyVulnerable);
        status.AddDamageModifier(GlobalModifiers.Instance.Get("Vulnerable"));
        ReleaseAttackSlot();
        flashController.FlashMultiple(5, 0.3f, 0.3f, 0.5f);
        yield return new WaitForSeconds(3f);

        animator.SetBool("IsStuck", false);

        yield return new WaitUntil(() => recoveryEnded == true);
        status.RemoveDamageModifier(GlobalModifiers.Instance.Get("Vulnerable"));
        SetState(EnemyState.Idle);
        attackInstance.release();
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
    private void ReleaseAttackSlot()
    {
        if (!hasAttackSlot)
            return;

        EnemyAttackManager.Release(this);
        hasAttackSlot = false;
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
        isStuck = false;
    }


    public void GetStuck()
    {
        isStuck = true;
    }


    public void ForbidMove()
    {
        canMove = false;
        RuntimeManager.PlayOneShot(step.nameIndex, transform.position);
    }

    public override void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (currentState != EnemyState.Attack)
            StartCoroutine(HookRoutine(origin, direction, offset, pullSpeed));
    }

    protected override void Die()
    {
        attack.hitbox.EndAttack();
        StopAllCoroutines();
        base.Die();
        animator.SetBool("IsDead", true);
        ReleaseAttackSlot();
    }
    protected override void OnPullStart()
    {
        animator.SetBool("IsHooked", true);
    }

    protected override void OnPullEnd()
    {
        animator.SetBool("IsHooked", false);
        base.OnPullEnd();
    }

    public override void Respawn()
    {
        animator.SetBool("IsDead", false);
        animator.Rebind();
        animator.Update(0f);
        base.Respawn();
    }
}