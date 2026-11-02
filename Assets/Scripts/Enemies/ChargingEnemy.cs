using System.Collections;
using System.Linq;
using FMODUnity;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChargingEnemy : BaseEnemy
{
    

    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;
    [Header("Prepare")]
    [SerializeField] private float prepareTime = 0.5f;

    [SerializeField] private SoundEvent agro;
    [SerializeField] private SoundEvent chase;
    
    [Header("Plunge")]
    [SerializeField] private float plungeSpeed = 12f;
    [SerializeField] private float curveAmplitude = 1f;
    [SerializeField] private float curveFrequency = 3f;
    [SerializeField] private AttackData attack;

    [Header("Recovery")]
    [SerializeField] private float recoverTime = 3f;


    private bool hitSolid;
    private bool windUpEnded;
    private Transform target;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private FlashController flash;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        flash = GetComponent<FlashController>();
        
    }
    private void Update()
    {

        if (currentState == EnemyState.Died)
            return;

        if (currentState != EnemyState.Idle)
            return;

        Collider2D player = Physics2D.OverlapCircle(
            transform.position,
            detectionRange,
            Layers.Player
        );

        if (player != null)
        {
            target = player.transform;
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        Debug.Log("fih");
        SetState(EnemyState.Preparing);
        Vector2 direction = ((Vector2)target.position - Rb.position).normalized;
        RefreshDirection(direction);
        animator.SetTrigger("WindUpStart");
        yield return new WaitUntil(() => EnemyAttackManager.TryAcquire(this));
        Debug.Log("fih1.5");
        var agroInstance = RuntimeManager.CreateInstance(agro.nameIndex);
        RuntimeManager.AttachInstanceToGameObject(agroInstance,gameObject);

        yield return new WaitUntil(() => windUpEnded);
        Debug.Log("fih2");
        windUpEnded = false;

        var chaseInstance = RuntimeManager.CreateInstance(chase.nameIndex);
        RuntimeManager.AttachInstanceToGameObject(chaseInstance,gameObject);

        animator.SetTrigger("AttackStart");
        yield return StartCoroutine(Plunge(target.position));
        chaseInstance.setParameterByName("HitSmth",1);
        
        EnemyAttackManager.Release(this);
        chaseInstance.release();
        animator.SetTrigger("AttackEnd");
        if (currentState != EnemyState.Vulnerable){
            SetState(EnemyState.Idle);
        }
        else{
            yield return new WaitForSeconds(recoverTime);
            SetState(EnemyState.Idle);
            status.RemoveDamageModifier(GlobalModifiers.Instance.Get("Vulnerable"));
        }
    }
    private IEnumerator Plunge(Vector2 targetPosition)
    {
        SetState(EnemyState.Attack);
        hitSolid = false;

        
        Vector2 direction = (targetPosition - Rb.position).normalized;
        RefreshDirection(direction);
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float time = 0f;

        attack.hitbox.BeginAttack((Vector2)transform.position - targetPosition, attack.damage, attack.knockback);
        while (!hitSolid && !attack.hitbox.hitObjects.Any())
        {
            time += Time.fixedDeltaTime;

            Vector2 movement = direction * plungeSpeed * Time.fixedDeltaTime;
            float curve = Mathf.Sin(time * curveFrequency) * curveAmplitude * Time.fixedDeltaTime;

            Rb.MovePosition(Rb.position + movement + perpendicular * curve);

            yield return new WaitForFixedUpdate();
        }
        attack.hitbox.EndAttack();
        hitSolid = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != EnemyState.Attack)
            return;

        if (((1 << other.gameObject.layer) & Layers.Walls) != 0)
        {
            hitSolid = true;
            SetState(EnemyState.Vulnerable);
            status.AddDamageModifier(GlobalModifiers.Instance.Get("Vulnerable"));
            flash.FlashMultiple(6,0.15f,0.25f,0.6f);
            
        }
    }

    protected override void Die()
    {
        attack.hitbox.EndAttack();
        StopAllCoroutines();
        base.Die();
        animator.SetBool("Died",true);
    }

    public void WindUpEnded()
    {
        windUpEnded = true;
    }

    private void RefreshDirection(Vector2 direction)
    {
        animator.SetFloat("VerticalDirection", direction.y);
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction.x < 0 ? -1 : 1);
        transform.localScale = scale;
    }
}