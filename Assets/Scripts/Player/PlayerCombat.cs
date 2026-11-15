using System.Collections;
using UnityEngine;
using FMODUnity;
using UnityEngine.Shaders;

public class PlayerCombat : MonoBehaviour
{
    private enum CombatState
    {
        Default,
        Attacking,

        Dashing,
        ChoosingTarget,
        Swinging,
        Pulling,
        ChoosingKickDirection
    }
    private CombatState currentState = CombatState.Default;

    [SerializeField] private PlayerComponents playerComponents;
    [SerializeField] private EventReference dashSound;
    [SerializeField] private EventReference castSound;
    [SerializeField] private EventReference reelSound;
    private Vector2 currentDirection = Vector2.right;

    [Header("Weapon")]
    [SerializeField] private Weapon equippedWeapon;
    [SerializeField] private Weapon equippedSecondaryWeapon;
    
    [Header("Utility")]
    [SerializeField] private UtilityItem equippedUtilityItem;
    [SerializeField] private float utilityTimeThreshold = 0.1f;

    private Rigidbody2D rb;
    private Coroutine currentUtilityCoroutine;

    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;

    [SerializeField] private Transform playerPivot;
    
    private bool canDash = true;
    public bool LockMove => currentState != CombatState.Default || externalActionLock;
    public bool CanAttack => currentState == CombatState.Default && !externalActionLock;
    private bool externalActionLock;
    [SerializeField] private Animator animator;

    public bool hasFishingRod = false;
    private void Update()
    {
        UpdateUtility();
        UpdateComboTimer();
    }

    private int comboIndex = 0;
    private float lastAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerComponents.inputHandler.AttackPressed += TryAttack;
        playerComponents.inputHandler.SecondaryAttackPressed += TrySecondaryAttack;
        playerComponents.inputHandler.UtilityPressed += TryStartUtility;
        playerComponents.inputHandler.UtilityReleased += TryFinishUtility;
        playerComponents.inputHandler.Dash += Dash;
        //Sub for the interrupt action
        playerComponents.controller.OnDamaged += InterruptUtility;
    }

    private void OnDisable()
    {
        playerComponents.controller.OnDamaged -= InterruptUtility;
        playerComponents.inputHandler.AttackPressed -= TryAttack;
        playerComponents.inputHandler.SecondaryAttackPressed -= TrySecondaryAttack;
        playerComponents.inputHandler.UtilityPressed -= TryStartUtility;
        playerComponents.inputHandler.UtilityReleased -= TryFinishUtility;
        playerComponents.inputHandler.Dash -= Dash;
        //Unsub from the interrupt action
    }

    private void TryAttack()
    {
        if (!CanAttack || !equippedWeapon || !playerComponents.status.isActiveAndEnabled)
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

        animator.SetFloat("attackIndex",comboIndex);
        animator.SetBool("IsAttacking",true);
        currentState = CombatState.Attacking;

        AttackData attack = weapon.GetAttack(comboIndex);


        if (attack == null)
        {
            currentState = CombatState.Default;
            yield break;
        }
        
        if (!attack.chargeSound.IsNull){
            RuntimeManager.PlayOneShot(attack.chargeSound);
        }
        GamepadVibration.Instance.Vibrate(attack.vibrationVelocity, attack.vibrationLength);
        yield return new WaitForSeconds(attack.chargeTime);
        RuntimeManager.PlayOneShot(attack.attackSound);
        
        currentDirection = playerComponents.inputHandler.LookDirection;
        Vector2 autolockDirection = TargetFinder.GetDirectionToClosestByAngle<BaseEnemy>(
            playerPivot.position,
            attack.autolockDistance,
            currentDirection,
            ~0,
            true
        );

        animator.SetFloat("DirX", currentDirection.x);
        animator.SetFloat("DirY", currentDirection.y);

        if (autolockDirection != Vector2.zero) currentDirection = autolockDirection;
        
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

        private void Dash()
    {
        
        if (currentState == CombatState.Default && canDash)
        {

            StartCoroutine(PerformDash());
        }
        
    }

    private IEnumerator PerformDash()
    {
        RuntimeManager.PlayOneShot(dashSound);
        canDash = false;
        currentDirection = playerComponents.inputHandler.MoveInput.normalized;
        currentState = CombatState.Dashing;
        rb.AddForce(currentDirection * dashForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        currentState = CombatState.Default;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void TryStartUtility()
    {
        if (currentState != CombatState.Default || !equippedUtilityItem || !equippedUtilityItem.isActiveAndEnabled)
        {
            return;
        }
        Debug.Log("TryStartUtility");
        UpdateUtility();
        currentState = CombatState.ChoosingTarget;
        equippedUtilityItem.ShowChosenTargetSprite();
    }
    
    private void TryFinishUtility()
    {
        if (currentState != CombatState.ChoosingTarget)
        {
            return;
        }

        bool isSmart;
        if (playerComponents.inputHandler.FishingRodTimePressed >= utilityTimeThreshold)
        {
            isSmart = true;
        }
        else
        { 
            isSmart = false;
        }
        
        currentUtilityCoroutine = StartCoroutine(Swing(isSmart));
    }
    private void UpdateComboTimer()
    {
        if (comboIndex <= 0)
            return;

        AttackData previousAttack = equippedWeapon.GetAttack(comboIndex - 1);

        if (previousAttack == null)
            return;

        if (Time.time - lastAttackTime > previousAttack.comboTime)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("attackIndex",0);
        }
    }
    private IEnumerator Swing(bool isSmart)
    {
        RuntimeManager.PlayOneShot(castSound);
        animator.SetTrigger("cast");
        currentState = CombatState.Swinging;
        yield return new WaitForSeconds(equippedUtilityItem.swingTime);
        equippedUtilityItem.Hook(isSmart);
        equippedUtilityItem.HideChosenTargetSprite();
        currentUtilityCoroutine = StartCoroutine(PullAnimation());
        
    }

    private IEnumerator PullAnimation()
    {
        animator.SetTrigger("pull");
        currentState = CombatState.Pulling;
        RuntimeManager.PlayOneShot(reelSound);
        yield return new WaitForSeconds(equippedUtilityItem.pullTime);
        currentState = CombatState.Default;
        currentUtilityCoroutine = null;
        animator.SetTrigger("endPull");
    }

    private void UpdateUtility()
    {
        if (currentState != CombatState.ChoosingTarget && currentState != CombatState.Swinging) return;
        bool isSmart = playerComponents.inputHandler.FishingRodTimePressed >= utilityTimeThreshold;
        equippedUtilityItem.UpdateChosenTargetSpritePosition(isSmart);
        
        if (currentState != CombatState.ChoosingTarget) return;
        if (playerComponents.inputHandler.AimDirection != Vector2.zero)
        {
            equippedUtilityItem.UpdateLookupDirection(playerComponents.inputHandler.AimDirection);
        }
    }
    
    private void InterruptUtility()
    {
        equippedUtilityItem.HideChosenTargetSprite();
        
        if (currentUtilityCoroutine != null)
        {
            StopCoroutine(currentUtilityCoroutine);
            currentUtilityCoroutine = null;
        }

        if (currentState == CombatState.Pulling
            || currentState == CombatState.Swinging
            || currentState == CombatState.ChoosingTarget)
        {
            currentState = CombatState.Default;
        }
    }
    
    public void SetExternalActionLock(bool locked)
    {
        externalActionLock = locked;
    }

    public void PickUpFishingRod()
    {
        hasFishingRod = true;
        this.equippedUtilityItem.gameObject.SetActive(true);
    }
}