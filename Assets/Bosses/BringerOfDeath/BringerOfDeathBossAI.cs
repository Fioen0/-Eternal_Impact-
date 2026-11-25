using System.Collections;
using UnityEngine;

/// <summary>Bringer of Death's basic boss loop. Special attacks can be added here later.</summary>
public class BringerOfDeathBossAI : MonoBehaviour
{
    [Header("Target and movement")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Min(0f)] private float detectRange = 12f;
    [SerializeField, Min(0f)] private float attackRange = 2.4f;
    [SerializeField, Min(0f)] private float moveSpeed = 1.6f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField, Min(0f)] private float groundCheckDistance = 0.2f;

    [Header("Basic attack")]
    [SerializeField, Min(0.1f)] private float attackDuration = 1f;
    [SerializeField, Min(0)] private int goldDropAmount = 100;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform visual;

    private Rigidbody2D body;
    private CharacterAttack characterAttack;
    private CharacterStats stats;
    private Animator animator;
    private Transform player;
    private bool isAttacking;
    private float attackPointDistance;
    private Vector3 visualBaseScale;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        characterAttack = GetComponent<CharacterAttack>();
        stats = GetComponent<CharacterStats>();

        animator = visual != null ? visual.GetComponent<Animator>() : GetComponentInChildren<Animator>();
        if (visual != null)
            visualBaseScale = visual.localScale;
        if (attackPoint != null)
            attackPointDistance = Mathf.Abs(attackPoint.localPosition.x);
    }

    private void OnEnable()
    {
        if (stats != null)
            stats.Died += HandleDeath;
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.Died -= HandleDeath;
    }

    private void Update()
    {
        if (stats != null && stats.isDead)
        {
            StopMoving();
            return;
        }

        if (isAttacking)
        {
            StopMoving();
            return;
        }

        FindPlayer();
        if (player == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectRange)
        {
            StopMoving();
            return;
        }

        FacePlayer();
        if (distance <= attackRange)
        {
            StopMoving();
            if (characterAttack != null && characterAttack.CanAttack())
                StartCoroutine(PerformAttack());
            return;
        }

        if (IsGrounded())
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            body.velocity = new Vector2(direction * moveSpeed, body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(0f, body.velocity.y);
        }

        if (animator != null)
            animator.SetBool("isMoving", Mathf.Abs(body.velocity.x) > 0.1f);
    }

    private void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (playerCollider == null)
        {
            player = null;
            return;
        }

        CharacterStats playerStats = playerCollider.GetComponentInParent<CharacterStats>();
        player = playerStats != null ? playerStats.transform : playerCollider.transform;
    }

    private bool IsGrounded()
    {
        return groundCheckPoint == null ||
               Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        if (direction == 0f)
            return;

        if (visual != null)
            visual.localScale = new Vector3(visualBaseScale.x * direction, visualBaseScale.y, visualBaseScale.z);
        if (attackPoint != null)
        {
            Vector3 position = attackPoint.localPosition;
            position.x = attackPointDistance * direction;
            attackPoint.localPosition = position;
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        StopMoving();

        if (animator != null)
            animator.SetTrigger("Attack");
        characterAttack.DoAttack();

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void StopMoving()
    {
        if (body != null)
            body.velocity = new Vector2(0f, body.velocity.y);
        if (animator != null)
            animator.SetBool("isMoving", false);
    }

    private void HandleDeath()
    {
        isAttacking = false;
        StopAllCoroutines();
        StopMoving();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetBool("isMoving", false);
        }

        if (goldDropAmount > 0)
            GoldCoinPickup.Spawn(transform.position, goldDropAmount);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }
}
