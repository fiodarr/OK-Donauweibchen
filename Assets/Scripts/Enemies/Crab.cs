using System.Collections;
using FMODUnity;
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
    [SerializeField] private AttackData attack, jumpAttack, shockwaveAttack;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject attackParticles, shockwaveParticles;

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float farJumpDistance = 5f;
    [SerializeField] private float farJumpSpeedMultiplier = 1.75f;

    [Header("Attack Switching")]
    [SerializeField] private float attackSwitchInterval = 2f;
    [SerializeField, Range(0f, 1f)] private float jumpSwitchChance = 0.35f;

    [Header("Armor Phases")]
    [SerializeField] private EntityStatus entityStatus;
    [SerializeField] private CrabHookPoint hookPoint;
    private float hookPointInitialX, hookPointInitialY;

    [SerializeField] private EventReference attackWindUpSound;
    [SerializeField] private EventReference attackSound;
    [SerializeField] private EventReference jumpWindUpSound;
    [SerializeField] private EventReference jumpSound;
    [SerializeField] private EventReference smashWindUpSound;
    [SerializeField] private EventReference smashSound;
    [SerializeField] private EventReference dieSound;
    [SerializeField] private EventReference walkSound;
    [SerializeField] private Transform arenaCenter;

    private bool hasMovedToArenaCenter;
    private bool isShockwaveSequence;

    private Transform target;
    private Vector2 direction;
    private Vector2 facingDirection;

    private bool canAttack = true;
    private bool windUpEnded, jumpEnded;
    private bool isJumpMoving;

    private int armorPhase;

    private CharacterSorting characterSorting;
    private Coroutine attackCoroutine;

    private float nextAttackSwitchTime;

    public bool HasArmor => entityStatus.GetArmor() > 0;

    private enum AttackType
    {
        Normal,
        Jump,
        Shockwave
    }

    private AttackType selectedAttack;
    private bool hasSelectedAttack;

    public Transform GetTarget()
    {
        return target;
    }

    protected override void Awake()
    {
        base.Awake();

        hookPointInitialX = hookPoint.transform.localPosition.x;
        hookPointInitialY = hookPoint.transform.localPosition.y;
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
            hasSelectedAttack = false;
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
        if (isShockwaveSequence)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

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

        if (!hasSelectedAttack)
            SelectNextAttack();

        // Immediately jump to the arena center when armor activates.
        if (HasArmor && !hasMovedToArenaCenter)
        {
            if (canAttack && attackCoroutine == null)
            {
                Rb.linearVelocity = Vector2.zero;
                animator.SetBool("IsMoving", false);

                attackCoroutine = StartCoroutine(JumpAttack());
            }

            return;
        }

        direction = ((Vector2)target.position - Rb.position).normalized;

        RefreshDirection(direction);

        float distance = Vector2.Distance(Rb.position, target.position);

        // While chasing with a normal attack, periodically try switching to Jump.
        if (!HasArmor &&
            selectedAttack == AttackType.Normal &&
            distance > attackRange &&
            Time.time >= nextAttackSwitchTime)
        {
            if (Random.value < jumpSwitchChance)
            {
                selectedAttack = AttackType.Jump;
            }
            else
            {
                nextAttackSwitchTime = Time.time + attackSwitchInterval;
            }
        }

        float currentAttackRange = attackRange;

        if (selectedAttack == AttackType.Jump)
        {
            currentAttackRange = attackRange * 2f;
        }

        if (selectedAttack == AttackType.Shockwave)
        {
            currentAttackRange = attackRange * 5f;
        }

        if (distance <= currentAttackRange)
        {
            Rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);

            if (canAttack && attackCoroutine == null)
            {
                switch (selectedAttack)
                {
                    case AttackType.Normal:
                        attackCoroutine = StartCoroutine(Attack());
                        break;

                    case AttackType.Jump:
                        attackCoroutine = StartCoroutine(JumpAttack());
                        break;

                    case AttackType.Shockwave:
                        attackCoroutine = StartCoroutine(SmashAttack());
                        break;
                }
            }

            return;
        }

        Rb.linearVelocity = direction * moveSpeed;
        animator.SetBool("IsMoving", true);
    }

    private void SelectNextAttack()
    {
        if (entityStatus.GetArmor() > 0)
        {
            if (!hasMovedToArenaCenter)
                selectedAttack = AttackType.Jump;
            else
                selectedAttack = AttackType.Shockwave;
        }
        else
        {
            hasMovedToArenaCenter = false;

            selectedAttack = Random.Range(0, 2) == 0
                ? AttackType.Normal
                : AttackType.Jump;

            nextAttackSwitchTime = Time.time + attackSwitchInterval;
        }

        hasSelectedAttack = true;
    }

    private void RefreshDirection(Vector2 direction)
    {
        facingDirection = direction.normalized;

        animator.SetFloat("DirY", direction.y);

        if (characterSorting != null)
            characterSorting.SetFacingDirection(direction.x);

        Vector3 position = hookPoint.transform.localPosition;

        position.x = direction.x > 0 ? -0.57f : -0.6f;
        position.y = direction.y > 0 ? 0f : 0f;

        hookPoint.transform.localPosition = position;
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
        {
            target = player.transform;
            hasSelectedAttack = false;
        }
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
        RuntimeManager.PlayOneShot(attackWindUpSound, transform.position);

        yield return new WaitUntil(() =>
            windUpEnded || GetState() != EnemyState.Preparing
        );

        if (GetState() != EnemyState.Preparing)
        {
            attackCoroutine = null;
            hasSelectedAttack = false;
            yield break;
        }

        windUpEnded = false;

        SetState(EnemyState.Attack);
        

        yield return new WaitUntil(() =>
            GetState() != EnemyState.Attack
        );

        yield return new WaitForSeconds(attack.recoveryTime);

        canAttack = true;
        attackCoroutine = null;
        hasSelectedAttack = false;

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

        Vector2 jumpTarget = entityStatus.GetArmor() > 0 && !hasMovedToArenaCenter
            ? (Vector2)arenaCenter.position
            : (Vector2)target.position;

        direction = (jumpTarget - Rb.position).normalized;
        RefreshDirection(direction);

        animator.SetTrigger("Jump");
        RuntimeManager.PlayOneShot(jumpWindUpSound, transform.position);

        yield return new WaitUntil(() =>
            windUpEnded || GetState() != EnemyState.Preparing
        );

        if (GetState() != EnemyState.Preparing)
        {
            attackCoroutine = null;
            hasSelectedAttack = false;
            yield break;
        }

        foreach (var hb in hurtbox)
        {
            hb.enabled = false;
        }

        windUpEnded = false;

        isJumpMoving = true;
        Coroutine movement = StartCoroutine(MoveTowardsTarget(jumpTarget));

        yield return new WaitUntil(() => jumpEnded);
        

        isJumpMoving = false;
        StopCoroutine(movement);

        foreach (var hb in hurtbox)
        {
            hb.enabled = true;
        }

        if (entityStatus.GetArmor() > 0)
            hasMovedToArenaCenter = true;

        SetState(EnemyState.Attack);

        yield return new WaitUntil(() =>
            GetState() != EnemyState.Attack
        );

        yield return new WaitForSeconds(jumpAttack.recoveryTime);

        canAttack = true;
        attackCoroutine = null;
        hasSelectedAttack = false;

        if (GetState() == EnemyState.Idle)
            yield break;
    }

    private IEnumerator SmashAttack()
    {
        canAttack = false;
        isShockwaveSequence = true;
        windUpEnded = false;

        SetState(EnemyState.Preparing);

        Rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);

        if (target != null)
        {
            direction = ((Vector2)target.position - Rb.position).normalized;
            RefreshDirection(direction);
        }

        for (int i = 0; i < 3; i++)
        {
            if (entityStatus.GetArmor() <= 0)
                break;

            windUpEnded = false;
            RuntimeManager.PlayOneShot(smashWindUpSound,transform.position);

            SetState(EnemyState.Preparing);
            animator.SetTrigger("Smash");

            yield return new WaitUntil(() =>
                windUpEnded || GetState() != EnemyState.Preparing
            );

            if (GetState() != EnemyState.Preparing)
                break;

            windUpEnded = false;

            
            SetState(EnemyState.Attack);

            yield return new WaitUntil(() =>
                GetState() != EnemyState.Attack
            );
        }

        Rb.linearVelocity = Vector2.zero;

        if (entityStatus.GetArmor() > 0)
            yield return new WaitForSeconds(shockwaveAttack.recoveryTime);

        isShockwaveSequence = false;
        canAttack = true;
        attackCoroutine = null;
        hasSelectedAttack = false;

        if (GetState() != EnemyState.Idle)
            yield break;
    }

    

    private IEnumerator MoveTowardsTarget(Vector2 tg)
    {
        while (!jumpEnded)
        {
            if (HasArmor && !hasMovedToArenaCenter)
                tg = arenaCenter.position;

            Vector2 toTarget = tg - Rb.position;
            float distance = toTarget.magnitude;

            if (distance <= 0.15f)
            {
                Rb.linearVelocity = Vector2.zero;
                yield return new WaitForFixedUpdate();
                continue;
            }

            direction = toTarget.normalized;

            float currentJumpSpeed = jumpSpeed;

            if (distance > farJumpDistance)
                currentJumpSpeed *= farJumpSpeedMultiplier;

            Rb.linearVelocity = direction * currentJumpSpeed;

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

    public void StartShockwave()
    {
        shockwaveAttack.hitbox.BeginAttack(
            direction,
            shockwaveAttack.damage,
            shockwaveAttack.knockback
        );

        if (shockwaveParticles != null)
        {
            Instantiate(
                shockwaveParticles,
                shockwaveAttack.hitbox.transform.position,
                Quaternion.identity
            );
        }
        RuntimeManager.PlayOneShot(smashSound, transform.position);
        CameraShake.Instance.Shake(0.2f, 0.2f);
        GamepadVibration.Instance.Vibrate(0.2f, 0.2f);
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
        RuntimeManager.PlayOneShot(attackSound, transform.position);
        CameraShake.Instance.Shake(0.05f, 0.05f);
        GamepadVibration.Instance.Vibrate(0.05f, 0.05f);
    }

    public void RemoveArmour()
    {
        Debug.Log("RemoveArmour");
        entityStatus.SetArmor(0);
    }

    public void StepSound()
    {
        RuntimeManager.PlayOneShot(walkSound,transform.position);
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
        RuntimeManager.PlayOneShot(jumpSound, transform.position);
        CameraShake.Instance.Shake(0.1f, 0.1f);
        GamepadVibration.Instance.Vibrate(0.1f, 0.1f);
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

    public override bool AllowedToHook()
    {
        return false;
    }
    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        float healthBefore = status.GetHealth();

        base.OnTakeDamage(damageDirection, damage, knockback);

        float healthAfter = status.GetHealth();

        if ((healthBefore > status.GetMaxHealth() * 0.66f &&
             healthAfter <= status.GetMaxHealth() * 0.66f) ||
            (healthBefore > status.GetMaxHealth() * 0.33f &&
             healthAfter <= status.GetMaxHealth() * 0.33f))
        {
            animator.SetTrigger("Hit");
            CameraShake.Instance.Shake(0.3f, 0.1f);
            GamepadVibration.Instance.Vibrate(0.3f, 0.3f);
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            attack.hitbox.EndAttack();
            jumpAttack.hitbox.EndAttack();
            shockwaveAttack.hitbox.EndAttack();

            canAttack = true;
            isJumpMoving = false;
            isShockwaveSequence = false;

            SetState(EnemyState.Idle);
        }
    }
    protected override void Die()
    {
        RuntimeManager.PlayOneShot(dieSound, transform.position);
        attack.hitbox.EndAttack();
        StopAllCoroutines();
        base.Die();
        animator.SetBool("IsDead", true);
    }
}