using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerController playerController;
    private Hitbox meleeHitbox;

    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float activeAttackTime = 0.15f;
    [SerializeField] private float recoveryTime = 0.25f;

    // placeholder
    [SerializeField] private float damage = 5f;

    public bool IsAttacking { get; private set; }

    private bool canAttack = true;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
        meleeHitbox = GetComponentInChildren<Hitbox>();
    }

    private void OnEnable()
    {
        inputHandler.AttackPressed += TryAttack;
    }

    private void OnDisable()
    {
        inputHandler.AttackPressed -= TryAttack;
    }

    private void TryAttack()
    {
        if (canAttack)
            StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;
        IsAttacking = true;

        Vector2 attackDirection = playerController.FacingDirection;

        float angle = Mathf.Atan2(
            attackDirection.y,
            attackDirection.x
        ) * Mathf.Rad2Deg;

        meleeHitbox.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        meleeHitbox.BeginAttack(attackDirection, damage);

        yield return new WaitForSeconds(activeAttackTime);

        meleeHitbox.EndAttack();

        yield return new WaitForSeconds(recoveryTime);

        IsAttacking = false;

        float remainingCooldown =
            Mathf.Max(0f, attackCooldown - activeAttackTime - recoveryTime);

        yield return new WaitForSeconds(remainingCooldown);

        canAttack = true;
    }
}