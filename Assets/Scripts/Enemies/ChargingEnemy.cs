using System.Collections;
using UnityEngine;

public class ChargingEnemy : BaseEnemy
{
    private enum EnemyState
    {
        Idle,
        Preparing,
        Plunging,
        Stunned
    }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;
    [Header("Prepare")]
    [SerializeField] private float prepareTime = 0.5f;

    [Header("Plunge")]
    [SerializeField] private float plungeSpeed = 12f;
    [SerializeField] private float curveAmplitude = 1f;
    [SerializeField] private float curveFrequency = 3f;

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

        yield return new WaitForSeconds(prepareTime);

        yield return StartCoroutine(Plunge(target.position));

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
        currentState = EnemyState.Plunging;
        hitSolid = false;

        Vector2 direction = (targetPosition - Rb.position).normalized;
        animator.SetFloat("VerticalDirection", direction.y);
        spriteRenderer.flipX = direction.x < 0;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float time = 0f;

        while (!hitSolid)
        {
            time += Time.fixedDeltaTime;

            Vector2 movement = direction * plungeSpeed * Time.fixedDeltaTime;
            float curve = Mathf.Sin(time * curveFrequency) * curveAmplitude * Time.fixedDeltaTime;

            Rb.MovePosition(Rb.position + movement + perpendicular * curve);

            yield return new WaitForFixedUpdate();
        }

        hitSolid = false;
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != EnemyState.Plunging)
            return;

        if (((1 << other.gameObject.layer) & Layers.Walls) != 0)
        {
            hitSolid = true;
            currentState = EnemyState.Stunned;
        }


        if (((1 << other.gameObject.layer) & Layers.Player) != 0)
        {
            hitSolid = true;
        }
    }
}