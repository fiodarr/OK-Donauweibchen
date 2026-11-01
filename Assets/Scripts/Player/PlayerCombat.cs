using System.Collections;
using UnityEngine;
using FMODUnity;

public class PlayerCombat : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerController playerController;
    private Vector2 currentDirection = Vector2.right;

    [Header("Weapon")]
    [SerializeField] private Weapon equippedWeapon;

    private Rigidbody2D rb;

    public bool IsAttacking { get; private set; }
    

    private bool canAttack = true;

    private int comboIndex = 0;
    private float lastAttackTime;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
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
        if (!canAttack || equippedWeapon == null)
            return;
        if (comboIndex > 0)
        {
            AttackData previousAttack = equippedWeapon.GetAttack(comboIndex - 1);

            if (previousAttack == null ||
                Time.time - lastAttackTime > previousAttack.comboTime)
            {
                comboIndex = 0;
            }
        }

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        canAttack = false;
        IsAttacking = true;

        AttackData attack = equippedWeapon.GetAttack(comboIndex);


        if (attack == null)
        {
            canAttack = true;
            IsAttacking = false;
            yield break;
        }
        if (inputHandler.MoveInput != Vector2.zero) 
            currentDirection = inputHandler.MoveInput.normalized;


        yield return new WaitForSeconds(attack.chargeTime);
        attack.attackSound.Play();
        
        // Launch player
        rb.AddForce(
            currentDirection * attack.launchForce,
            ForceMode2D.Impulse
        );

        // Rotate hitbox
        float angle = Mathf.Atan2(
            currentDirection.y,
            currentDirection.x
        ) * Mathf.Rad2Deg;

        attack.hitbox.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Execute attack
        attack.hitbox.BeginAttack(currentDirection, attack.damage, attack.knockback);

        yield return new WaitForSeconds(attack.activeTime);

        attack.hitbox.EndAttack();

        yield return new WaitForSeconds(attack.recoveryTime);

        IsAttacking = false;

        lastAttackTime = Time.time;

        // Advance combo
        comboIndex++;

        if (comboIndex >= equippedWeapon.attacks.Length)
            comboIndex = 0;

        float remainingCooldown = Mathf.Max(
            0f,
            equippedWeapon.attackCooldown -
            attack.activeTime -
            attack.recoveryTime
        );

        yield return new WaitForSeconds(remainingCooldown);

        canAttack = true;
    }
}