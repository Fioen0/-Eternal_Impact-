using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int defense = 0; // 방어력도 미리 넣어두면 좋죠!

    // ✨ 외부에 데이터만 쏙 전달해 주는 프로퍼티 (수정은 내부에서만 가능!)
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public int AttackDamage => attackDamage;
    public int Defense => defense;

    // ✨ 무적 상태 플래그 (플레이어 대쉬, 몹 피격 후 무적 시간 등에 공용 활용 가능)
    public bool isInvincible { get; set; } = false;

    [Header("UI 연동 (선택사항)")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject damagePopupPrefab;

    void Awake()
    {
        currentHp = maxHp;
    }

    void Start()
    {
        if (healthBar != null) healthBar.UpdateHealthBar(currentHp, maxHp);
    }

    // ✨ 데미지 받기 (공용)
    public void TakeDamage(int incomingDamage)
    {
        if (isInvincible) return;

        // 방어력 계산 적용 (최소 1 데미지는 보장)
        int finalDamage = Mathf.Max(incomingDamage - defense, 1);
        currentHp -= finalDamage;

        // 체력바 갱신
        if (healthBar != null) healthBar.UpdateHealthBar(currentHp, maxHp);

        // 데미지 팝업 소환
        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 1.0f, 0);
            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            FloatingText floatingText = popup.GetComponent<FloatingText>();
            if (floatingText != null) floatingText.SetDamage(finalDamage);
        }

        if (currentHp <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        Destroy(gameObject);
    }
}