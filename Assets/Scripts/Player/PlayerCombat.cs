using System.Collections;
using UnityEngine;
using FMODUnity;

public class PlayerCombat : MonoBehaviour
{
    private enum CombatState
    {
        Default,
        Attacking,
        ChoosingTarget,
        Pulling
    }
    private CombatState currentState = CombatState.Default;
    
    private PlayerInputHandler inputHandler;
    private PlayerController playerController;
    private Vector2 currentDirection = Vector2.right;

    [Header("Weapon")]
    [SerializeField] private Weapon equippedWeapon;
    [SerializeField] private Weapon equippedSecondaryWeapon;
    
    [Header("Utility")]
    [SerializeField] private UtilityItem equippedUtilityItem;
    [SerializeField] private float utilityTimeThreshold = 0.1f;

    private Rigidbody2D rb;

    public bool LockMove
    {
        get
        {
            return currentState != CombatState.Default;
        }
    }

    private void Update()
    {
        UpdateUtility();
    }

    private bool CanAttack
    {
        get
        {
            return currentState == CombatState.Default;
        }
    }

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
        inputHandler.SecondaryAttackPressed += TrySecondaryAttack;
        inputHandler.UtilityPressed += TryStartUtility;
        inputHandler.UtilityReleased += TryFinishUtility;
    }

    private void OnDisable()
    {
        inputHandler.AttackPressed -= TryAttack;
        inputHandler.SecondaryAttackPressed -= TrySecondaryAttack;
        inputHandler.UtilityPressed -= TryStartUtility;
        inputHandler.UtilityReleased -= TryFinishUtility;
    }

    private void TryAttack()
    {
        if (!CanAttack || !equippedWeapon)
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

        StartCoroutine(PerformAttack(equippedWeapon));
    }

        private void TrySecondaryAttack()
    {
        if (!CanAttack || !equippedSecondaryWeapon)
            return;
        if (comboIndex > 0)
        {
            AttackData previousAttack = equippedSecondaryWeapon.GetAttack(comboIndex - 1);

            if (previousAttack == null ||
                Time.time - lastAttackTime > previousAttack.comboTime)
            {
                comboIndex = 0;
            }
        }

        StartCoroutine(PerformAttack(equippedSecondaryWeapon));
    }

    private IEnumerator PerformAttack(Weapon weapon)
    {
        currentState = CombatState.Attacking;

        AttackData attack = weapon.GetAttack(comboIndex);


        if (attack == null)
        {
            currentState = CombatState.Default;
            yield break;
        }
        if (inputHandler.MoveInput != Vector2.zero) 
            currentDirection = inputHandler.MoveInput.normalized;
        if (attack.chargeSound!=null){
            attack.chargeSound.Play();
        }
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

        lastAttackTime = Time.time;

        // Advance combo
        comboIndex++;

        if (comboIndex >= weapon.attacks.Length)
            comboIndex = 0;

        currentState = CombatState.Default;
    }

    private void TryStartUtility()
    {
        if (currentState != CombatState.Default || !equippedUtilityItem)
        {
            
            return;
        }
        Debug.Log("TryStartUtility");
        currentState = CombatState.ChoosingTarget;
        //Start updating rotation inside the utility
        
    }
    
    private void TryFinishUtility()
    {
        if (currentState != CombatState.ChoosingTarget)
        {
            return;
        }

        if (inputHandler.FishingRodTimePressed >= utilityTimeThreshold)
        {
            equippedUtilityItem.HookSmart(inputHandler.MoveInput); //TODO: Distinguish mouse for mouse and keyboard and move direction for gamepad
        }
        else
        {
            equippedUtilityItem.HookDefault();
        }
        
        currentState = CombatState.Default;
        
    }

    private void UpdateUtility()
    {
        if (currentState != CombatState.ChoosingTarget) return;
        if (inputHandler.MoveInput != Vector2.zero) 
            currentDirection = inputHandler.MoveInput.normalized;
        equippedUtilityItem.UpdateLookupDirection(currentDirection);
    }
    
}