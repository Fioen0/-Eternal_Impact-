using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("공격력 설정")]
    [SerializeField] private int attackDamage = 10;

    [Header("무적 설정")]
    [SerializeField] private float invincibleDuration = 1.0f; // 피격 후 무적 시간 (초)
    
    // 외부 접근용 변수 및 프로퍼티
    public bool isInvincible = false; 
    public bool isDead { get; private set; } = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackDamage => attackDamage;

    // ✨ [추가] 에러를 해결하기 위한 내부 참조 컴포넌트 선언
    private Collider2D playerCollider;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = maxHealth;

        // ✨ [추가] 게임 시작 시 컴포넌트를 자동으로 할당받아 에러 방지
        playerCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // 데미지 입기
    public void TakeDamage(int damage)
    {
        // 이미 죽었거나 무적 상태일 경우 데미지 무시
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 데미지를 입은 후 일시적 무적 상태 돌입
            StartCoroutine(BecomeInvincible());
        }
    }

    // 피격 시 일시적 무적 코루틴
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;

        // ✨ 1. 더 이상 적에게 맞거나 감지되지 않도록 Collider 끄기
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // ✨ 2. 물리 연산 중단 (제자리 정지)
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false; // 물리 연산 완전히 비활성화
        }

        // ✨ 3. 사망 애니메이션 실행 (애니메이터가 있을 때만 안전하게 실행)
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // ✨ 4. 사망 1.5초 후 캐릭터 오브젝트를 삭제
        Destroy(gameObject, 1.5f);
    }
}