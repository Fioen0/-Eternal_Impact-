using System.Collections;
using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("감지와 이동") ]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField, Min(0f)] private float detectRange = 8f;
    [SerializeField, Min(0f)] private float attackRange = 5f;
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    [Header("투사체 공격") ]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0f)] private float projectileSpeed = 5f;
    [SerializeField, Min(0.1f)] private float attackCooldown = 4.5f;
    [SerializeField, Min(0f)] private float attackWindupDuration = 0.65f;
    [SerializeField, Min(0.1f)] private float attackAnimationDuration = 0.8f;
    [SerializeField, Min(0)] private int goldDropAmount = 10;

    private Rigidbody2D body;
    private Animator animator;
    private CharacterStats characterStats;
    private Transform player;
    private Transform queuedTarget;
    private Vector2 queuedDirection;
    private float nextAttackTime;
    private bool isAttacking;
    private GameObject activeTelegraph;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void OnEnable()
    {
        if (characterStats != null)
            characterStats.Died += HandleDeath;
    }

    private void OnDisable()
    {
        if (characterStats != null)
            characterStats.Died -= HandleDeath;
    }

    private void Update()
    {
        if (characterStats != null && characterStats.isDead)
        {
            if (body != null) body.velocity = Vector2.zero;
            return;
        }

        if (isAttacking)
        {
            body.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        FindPlayer();
        if (player == null)
        {
            body.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - body.position;
        FacePlayer(toPlayer.x);
        if (toPlayer.magnitude > attackRange)
        {
            body.velocity = toPlayer.normalized * moveSpeed;
            if (animator != null) animator.SetBool("isMoving", body.velocity.sqrMagnitude > 0.01f);
            return;
        }

        body.velocity = Vector2.zero;
        if (animator != null) animator.SetBool("isMoving", false);
        if (Time.time >= nextAttackTime)
            BeginAttackWarning(toPlayer);
    }

    private void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        player = playerCollider != null ? playerCollider.transform : null;
    }

    private void FacePlayer(float horizontalDirection)
    {
        if (Mathf.Abs(horizontalDirection) < 0.01f) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(horizontalDirection);
        transform.localScale = scale;
    }

    private void BeginAttackWarning(Vector2 direction)
    {
        nextAttackTime = Time.time + attackCooldown;
        queuedTarget = null;
        queuedDirection = direction;
        isAttacking = true;

        Vector3 warningPosition = firePoint != null
            ? firePoint.position
            : transform.position + Vector3.up * 0.6f;
        SpriteRenderer sprite = animator != null ? animator.GetComponent<SpriteRenderer>() : null;
        activeTelegraph = EnemyAttackTelegraph.Create(warningPosition, transform,
            sprite != null ? sprite.sortingLayerID : 0,
            sprite != null ? sprite.sortingOrder + 1 : 1);
        StartCoroutine(PerformAttackAfterWarning());
    }

    private IEnumerator PerformAttackAfterWarning()
    {
        yield return EnemyAttackTelegraph.Pulse(activeTelegraph, attackWindupDuration);
        activeTelegraph = null;

        if (characterStats != null && characterStats.isDead)
        {
            isAttacking = false;
            yield break;
        }

        if (animator != null)
            animator.SetTrigger("Attack");
        else
            FireProjectileFromAnimation();

        yield return new WaitForSeconds(attackAnimationDuration);
        isAttacking = false;
    }

    public void FireProjectileFromAnimation()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"[{name}] Projectile Prefab이 연결되지 않았습니다.", this);
            return;
        }

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = queuedTarget != null
            ? (Vector2)queuedTarget.position - (Vector2)origin
            : queuedDirection;
        if (direction.sqrMagnitude < 0.001f)
            direction = transform.localScale.x < 0 ? Vector2.left : Vector2.right;

        EnemyProjectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.Launch(direction, projectileSpeed);
        queuedTarget = null;
    }

    private void HandleDeath()
    {
        StopAllCoroutines();
        isAttacking = false;
        if (activeTelegraph != null) Destroy(activeTelegraph);
        activeTelegraph = null;
        if (body != null) body.velocity = Vector2.zero;
        if (animator != null) animator.SetBool("isMoving", false);
        if (goldDropAmount > 0)
            GoldCoinPickup.Spawn(transform.position, goldDropAmount);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
