using System.Collections;
using System.Linq;
using FMODUnity;
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

    private EnemyState currentState = EnemyState.Idle;
    private Transform target;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    private void Update()
    {
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
        currentState = EnemyState.Preparing;
        
        yield return new WaitUntil(() => EnemyAttackManager.TryAcquire(this));
        agro.PlayOnGameObject(gameObject);

        yield return new WaitForSeconds(prepareTime);

        chase.PlayOnGameObject(gameObject);
        yield return StartCoroutine(Plunge(target.position));
        chase.SetParameter("HitSmth",1);
        
        EnemyAttackManager.Release(this);
        chase.End();
        if (currentState != EnemyState.Stunned){
            currentState = EnemyState.Idle;
        }
        else{
            yield return new WaitForSeconds(recoverTime);
            currentState = EnemyState.Idle;
        }
    }
    private IEnumerator Plunge(Vector2 targetPosition)
    {
        currentState = EnemyState.Attack;
        hitSolid = false;

        Vector2 direction = (targetPosition - Rb.position).normalized;
        animator.SetFloat("VerticalDirection", direction.y);
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction.x < 0 ? -1 : 1);
        transform.localScale = scale;

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
            currentState = EnemyState.Stunned;
        }
    }
}