using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    // ✨ int 타입(현재 체력, 최대 체력)을 전달하는 이벤트로 표준화!
    // HeartListUI와 HealthBar 모두 지원 가능합니다.
    [Header("UI 연동 이벤트")]
    public UnityEvent<int, int> OnHealthChanged;

    private Collider2D playerCollider;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = maxHealth;

        playerCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 시작 시 초기 체력 신호 전송
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // 데미지 입기
    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}/{maxHealth}");

        // 체력 변경 이벤트 호출 (UI 업데이트 신호)
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;

        if (playerCollider != null) playerCollider.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (anim != null) anim.SetTrigger("Die");

        Destroy(gameObject, 1.5f);
    }
}