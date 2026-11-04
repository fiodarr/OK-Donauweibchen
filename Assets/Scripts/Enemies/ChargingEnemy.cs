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

    [Header("Wall Avoidance")]
    [SerializeField] private float avoidanceDuration = 1f;
    [SerializeField] private float avoidanceDistance = 2f;
    [SerializeField] private float avoidanceStrength = 3f;

    [SerializeField] private SoundEvent agro;
    [SerializeField] private SoundEvent chase;
    
    [Header("Plunge")]
    [SerializeField] private float plungeSpeed = 12f;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float curveAmplitude = 1f;
    [SerializeField] private float curveFrequency = 3f;
    [SerializeField] private AttackData attack;

    [Header("Recovery")]
    [SerializeField] private float recoverTime = 3f;


    private bool hitSolid;


    private bool hitWallWhileMoving;
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
            if (Vector2.Distance(transform.position,target.position) > 6f || hitSolid)
            {

                animator.SetBool("IsMoving",false);
                StartCoroutine(Attack());
            }
            else
            {
                animator.SetBool("IsMoving",true);
                Vector2 direction = -((Vector2)target.position - Rb.position).normalized;
                RefreshDirection(direction);

                direction = AvoidWalls(direction);
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);
                Vector2 movement = direction * moveSpeed * Time.fixedDeltaTime;
                float curve = Mathf.Sin(Time.time * curveFrequency) * curveAmplitude * Time.deltaTime;

                Rb.MovePosition(Rb.position + movement + perpendicular * curve);
            }
        }
    }

    private IEnumerator Attack()
    {
        
        SetState(EnemyState.Preparing);
        Vector2 direction = ((Vector2)target.position - Rb.position).normalized;
        RefreshDirection(direction);
        animator.SetTrigger("WindUpStart");
        yield return new WaitUntil(() => EnemyAttackManager.TryAcquire(this));
    
        RuntimeManager.PlayOneShot(agro.nameIndex, transform.position);

        yield return new WaitUntil(() => windUpEnded);
        
        windUpEnded = false;

        var chaseInstance = RuntimeManager.CreateInstance(chase.nameIndex);
        RuntimeManager.AttachInstanceToGameObject(chaseInstance,gameObject);
        chaseInstance.start();

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
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        RefreshDirection(direction);

        float time = 0f;

        attack.hitbox.BeginAttack(
            (Vector2)transform.position - targetPosition,
            attack.damage,
            attack.knockback
        );

        while (!hitSolid && !attack.hitbox.hitObjects.Any())
        {
            time += Time.fixedDeltaTime;

            if (time <= avoidanceDuration)
            {
                direction = AvoidWalls(direction);
                perpendicular = new Vector2(-direction.y, direction.x);
                RefreshDirection(direction);
            }

            Vector2 movement = direction * plungeSpeed * Time.fixedDeltaTime;
            float curve = Mathf.Sin(time * curveFrequency) *
                        curveAmplitude * Time.fixedDeltaTime;

            Rb.MovePosition(Rb.position + movement + perpendicular * curve);

            yield return new WaitForFixedUpdate();
        }

        attack.hitbox.EndAttack();
    }
    private Vector2 AvoidWalls(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            Rb.position,
            direction,
            avoidanceDistance,
            Layers.Walls
        );

        if (hit.collider == null)
            return direction;

        Vector2 slideDirection = direction - Vector2.Dot(direction, hit.normal) * hit.normal;

        return slideDirection.normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {


        if (((1 << other.gameObject.layer) & Layers.Walls) != 0)
        {

            hitSolid = true;
            if (currentState != EnemyState.Attack)
                return;
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