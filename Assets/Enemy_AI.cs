using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Idle,   // 대기
        Chase,  // 추적
        Attack  // 공격
    }

    [Header("상태 설정")]
    [SerializeField] private State currentState = State.Idle;

    [Header("사거리 및 속도 설정")]
    [SerializeField] private float detectRange = 6f;   // 플레이어 감지 거리
    [SerializeField] private float attackRange = 1.2f;  // 공격 사거리
    [SerializeField] private float moveSpeed = 2.5f;    // 이동 속도

    [Header("공격 딜레이 설정")]
    [SerializeField] private float attackDuration = 0.8f; // ✨ 공격 모션 시간 (초)

    [Header("타겟 및 레이어 설정")]
    [SerializeField] private LayerMask playerLayer;   // 플레이어 레이어
    [SerializeField] private LayerMask groundLayer;   // 바닥 레이어
    private Transform playerTransform;

    [Header("바닥 감지 (Ground Check)")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistance = 0.2f;
    private bool isGrounded;

    // 공격 상태 플래그
    private bool isAttacking = false;

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private CharacterAttack enemyAttack;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAttack = GetComponent<CharacterAttack>();

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // ✨ [핵심 1] 공격 중일 때는 속도를 0으로 강제 고정하고 아래 로직을 전부 건너뜁니다.
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 1. 바닥 및 플레이어 감지
        CheckGround();
        FindPlayer();

        // 2. 상태 결정
        if (playerTransform == null)
        {
            currentState = State.Idle;
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attack;
            }
            else if (distanceToPlayer <= detectRange)
            {
                currentState = State.Chase;
            }
            else
            {
                currentState = State.Idle;
            }
        }

        // 3. 상태별 행동 수행
        HandleState();

        // 4. 애니메이션 상태 업데이트
        UpdateAnimation();
    }

    private void CheckGround()
    {
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        }
        else
        {
            isGrounded = true; 
        }
    }

    private void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        playerTransform = (playerCollider != null) ? playerCollider.transform : null;
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                rb.velocity = new Vector2(0f, rb.velocity.y);
                break;

            case State.Chase:
                MoveToPlayer();
                break;

            case State.Attack:
                LookAtPlayer();
                
                // 공격 가능할 때 코루틴 실행
                if (enemyAttack != null && enemyAttack.CanAttack())
                {
                    StartCoroutine(PerformAttackCoroutine());
                }
                break;
        }
    }

    // ✨ [핵심 2] X축 이동 위치를 아예 물리적으로 고정시키는 공격 코루틴
    private IEnumerator PerformAttackCoroutine()
    {
        isAttacking = true;
        
        // 이동 및 애니메이션 강제 정지
        rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("isMoving", false);
            anim.SetTrigger("Attack");
        }

        // X축 위치 고정 (움직임 물리 락)
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        enemyAttack.DoAttack();

        // 공격 애니메이션이 진행되는 동안 대기
        yield return new WaitForSeconds(attackDuration);

        // 위치 고정 해제 (원래대로 복구)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isAttacking = false;
    }

    private void MoveToPlayer()
    {
        if (playerTransform == null || !isGrounded) return;

        float direction = playerTransform.position.x > transform.position.x ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (playerTransform == null) return;

        if (playerTransform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // 왼쪽 바라보기
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);  // 오른쪽 바라보기
        }
    }

    private void UpdateAnimation()
    {
        if (anim == null || isAttacking) return;

        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        anim.SetBool("isMoving", isMoving);
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