using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private Transform attackPoint;      // 공격 위치
    [SerializeField] private float attackRange = 0.5f;     // 공격 범위 반지름
    [SerializeField] private float attackCooldown = 1.0f;  // 공격 쿨타임 (초)
    [SerializeField] private LayerMask targetLayer;       // 타겟(플레이어 또는 적) 레이어

    [Header("이펙트 설정")]
    [SerializeField] private GameObject attackEffectPrefab; // 공격 이펙트

    private float nextAttackTime = 0f; // 다음 공격 가능 시간
    private CharacterStats myStats;    // 스탯 참조

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
    }

    // EnemyAI.cs에서 호출할 수 있도록 public 함수를 추가합니다!
    public bool CanAttack()
    {
        // 현재 시간이 다음 공격 가능 시간보다 크거나 같으면 true 반환
        return Time.time >= nextAttackTime;
    }

    public void DoAttack()
    {
        if (!CanAttack()) return;

        // 쿨타임 갱신
        nextAttackTime = Time.time + attackCooldown;
    }
    
    public void HitCheck()
    {
        if (attackPoint == null) return;

        // 이펙트 생성
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);
        }

        // 범위 내 타겟 감지 및 데미지 전달
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
        int myDamage = (myStats != null) ? myStats.AttackDamage : 10;

        foreach (Collider2D target in hitTargets)
        {
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats != null)
            {
                targetStats.TakeDamage(myDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}