using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : MonoBehaviour
{
    public event Action Died;
    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private int bonusMaxHealth;

    [Header("공격력 설정")]
    [SerializeField] private int attackDamage = 10;
    private int bonusAttackDamage;

    [Header("무적 설정")]
    [SerializeField] private float invincibleDuration = 1.0f; // 피격 후 무적 시간 (초)
    
    // 외부 접근용 변수 및 프로퍼티
    public bool isInvincible = false; 
    public bool isDead { get; private set; } = false;

    public int MaxHealth => Mathf.Max(1, maxHealth + bonusMaxHealth);
    public int CurrentHealth => currentHealth;
    public int AttackDamage => Mathf.Max(0, attackDamage + bonusAttackDamage);

    // ✨ int 타입(현재 체력, 최대 체력)을 전달하는 이벤트로 표준화!
    // HeartListUI와 HealthBar 모두 지원 가능합니다.
    [Header("UI 연동 이벤트")]
    public UnityEvent<int, int> OnHealthChanged;

    private Collider2D playerCollider;
    private Animator anim;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = MaxHealth;

        playerCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // 플레이어에만 장비 시스템을 보장합니다. 적의 CharacterStats에는 추가하지 않습니다.
        if (CompareTag("Player") && GetComponent<EquipmentManager>() == null)
            gameObject.AddComponent<EquipmentManager>();
    }

    public void SetEquipmentBonuses(int maxHealthBonus, int attackDamageBonus)
    {
        int previousMaxHealth = MaxHealth;
        bonusMaxHealth = maxHealthBonus;
        bonusAttackDamage = attackDamageBonus;
        int maxHealthChange = MaxHealth - previousMaxHealth;

        // 최대 체력이 오른 만큼 현재 체력도 채웁니다. 해제 시에는 새 최대치로만 제한합니다.
        currentHealth = maxHealthChange > 0
            ? Mathf.Min(currentHealth + maxHealthChange, MaxHealth)
            : Mathf.Clamp(currentHealth, 0, MaxHealth);

        if (previousMaxHealth != MaxHealth)
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    void Start()
    {
        // 시작 시 초기 체력 신호 전송
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    // 데미지 입기
    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible || damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}/{MaxHealth}");

        // 체력 변경 이벤트 호출 (UI 업데이트 신호)
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvincible());
        }
    }

    // 체력 변경의 단일 기준점. 회복량이 실제로 적용된 경우에만 true를 반환합니다.
    public bool Heal(int amount)
    {
        if (isDead || amount <= 0 || currentHealth >= MaxHealth) return false;

        int previousHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
        Debug.Log($"{gameObject.name} 회복! 체력: {previousHealth} → {currentHealth}/{MaxHealth}");
        return currentHealth > previousHealth;
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
        Died?.Invoke();

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
