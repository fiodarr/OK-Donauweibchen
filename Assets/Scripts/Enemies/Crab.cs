
using System.Collections;
using UnityEngine;

public class Crab : BaseEnemy
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float loseTargetRange = 15f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Transform detectionPivot;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attack")]
    [SerializeField] private Animator animator;
    [SerializeField] private AttackData attack, jumpAttack;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject attackParticles;

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 5f;

    [Header("Armor Phases")]
    [SerializeField] private EntityStatus entityStatus;
    [SerializeField] private CrabHookPoint hookPoint;

    private Transform target;
    private Vector2 direction;
    private Vector2 facingDirection;

    private bool canAttack = true;
    private bool windUpEnded, jumpEnded;
    private bool isJumpMoving;

    private int armorPhase;

    private CharacterSorting characterSorting;
    private Coroutine attackCoroutine;

    protected override void Awake()
    {
        base.Awake();

        characterSorting = GetComponent<CharacterSorting>();
    }

    private void Update()
    {
        CheckArmorPhase();

        if (GetState() != EnemyState.Idle)
            return;

        if (target == null)
        {
            DetectPlayer();
            return;
        }

        if (Vector2.Distance(detectionPivot.position, target.position) > loseTargetRange)
        {
            target = null;
        }
    }

    private void CheckArmorPhase()
    {
        float hpPercent = entityStatus.GetHealth() / entityStatus.GetMaxHealth();

        if (armorPhase == 0 && hpPercent <= 0.66f)
        {
            entityStatus.SetArmor(999999);
            armorPhase = 1;
        }
        else if (armorPhase == 1 && hpPercent <= 0.33f)
        {
            entityStatus.SetArmor(999999);
            armorPhase = 2;
        }
    }

    private void FixedUpdate()
    {
        if (isJumpMoving)
            return;

        if (GetState() != EnemyState.Idle)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        if (target == null)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        direction = ((Vector2)target.position - Rb.position).normalized;

        RefreshDirection(direction);

        float distance = Vector2.Distance(Rb.position, target.position);

        if (distance <= attackRange * 5)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);

            if (canAttack && attackCoroutine == null)
                attackCoroutine = StartCoroutine(JumpAttack());

            return;
        }

        Rb.linearVelocity = direction * moveSpeed;
        animator.SetBool("IsMoving", true);
    }

    private void RefreshDirection(Vector2 direction)
    {
        facingDirection = direction.normalized;
        
        animator.SetFloat("DirY", direction.y);

        if (characterSorting != null)
            characterSorting.SetFacingDirection(direction.x);
    }

    public bool IsPointBehind(Vector2 point)
    {
        Vector2 directionToPoint = (point - (Vector2)transform.position).normalized;

        float dot = Vector2.Dot(facingDirection, directionToPoint);

        return dot < -0.5f;
    }

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

    private IEnumerator Attack()
    {
        canAttack = false;
        windUpEnded = false;

        SetState(EnemyState.Preparing);

        Rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);

        if (target != null)
        {
            direction = ((Vector2)target.position - Rb.position).normalized;
            RefreshDirection(direction);
        }

        animator.SetTrigger("Attack");

        yield return new WaitUntil(() =>
            windUpEnded || GetState() != EnemyState.Preparing
        );

        if (GetState() != EnemyState.Preparing)
        {
            attackCoroutine = null;
            yield break;
        }

        windUpEnded = false;

        SetState(EnemyState.Attack);

        yield return new WaitUntil(() =>
            GetState() != EnemyState.Attack
        );

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        attackCoroutine = null;

        if (GetState() == EnemyState.Idle)
            yield break;
    }

    private IEnumerator JumpAttack()
    {
        canAttack = false;
        windUpEnded = false;
        jumpEnded = false;

        SetState(EnemyState.Preparing);

        Rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);

        if (target != null)
        {
            direction = ((Vector2)target.position - Rb.position).normalized;
            RefreshDirection(direction);
        }

        animator.SetTrigger("Jump");

        yield return new WaitUntil(() =>
            windUpEnded || GetState() != EnemyState.Preparing
        );

        if (GetState() != EnemyState.Preparing)
        {
            attackCoroutine = null;
            yield break;
        }

        foreach (var hb in hurtbox)
        {
            hb.enabled = false;
        }

        windUpEnded = false;

        isJumpMoving = true;
        Coroutine movement = StartCoroutine(MoveTowardsTarget(target.position));

        yield return new WaitUntil(() => jumpEnded);

        isJumpMoving = false;
        StopCoroutine(movement);

        foreach (var hb in hurtbox)
        {
            hb.enabled = true;
        }

        SetState(EnemyState.Attack);

        yield return new WaitUntil(() =>
            GetState() != EnemyState.Attack
        );

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        attackCoroutine = null;

        if (GetState() == EnemyState.Idle)
            yield break;
    }

    private IEnumerator MoveTowardsTarget(Vector2 tg)
    {
        while (!jumpEnded)
        {
            direction = (tg - Rb.position).normalized;

            //RefreshDirection(direction);

            Rb.linearVelocity = direction * jumpSpeed;

            yield return new WaitForFixedUpdate();
        }

        Rb.linearVelocity = Vector2.zero;
    }

    public void WindUpEnded()
    {
        if (GetState() == EnemyState.Preparing)
            windUpEnded = true;
    }

    public void JumpEnded()
    {
        if (GetState() == EnemyState.Preparing)
            jumpEnded = true;
    }

    public void ActivateHitbox()
    {
        attack.hitbox.BeginAttack(
            direction,
            attack.damage,
            attack.knockback
        );

        if (attackParticles != null)
        {
            Instantiate(
                attackParticles,
                attack.hitbox.transform.position,
                Quaternion.identity
            );
        }
    }

    public void RemoveArmour()
    {
        Debug.Log("RemoveArmour");
        entityStatus.SetArmor(0);
    }

    public void ActivateJumpHitbox()
    {
        jumpAttack.hitbox.BeginAttack(
            direction,
            jumpAttack.damage,
            jumpAttack.knockback
        );

        if (attackParticles != null)
        {
            Instantiate(
                attackParticles,
                jumpAttack.hitbox.transform.position,
                Quaternion.identity
            );
        }
    }

    public void DeactivateHitbox()
    {
        attack.hitbox.EndAttack();
    }

    public void DeactivateJumpHitbox()
    {
        jumpAttack.hitbox.EndAttack();
    }

    public void AttackEnd()
    {
        if (GetState() == EnemyState.Attack)
            SetState(EnemyState.Idle);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pivot = detectionPivot != null
            ? detectionPivot.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivot, loseTargetRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot, attackRange);
    }
}
