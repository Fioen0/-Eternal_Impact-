using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private Transform attackPoint;      // 공격 판정 위치
    [SerializeField] private float attackRange = 0.5f;     // 공격 범위 반지름
    [SerializeField] private float attackCooldown = 1.0f;  // 공격 쿨타임 (초)
    [SerializeField] private LayerMask targetLayer;       // 공격 대상 레이어 (플레이어는 Enemy, 적은 Player)

    [Header("이펙트 설정")]
    [SerializeField] private GameObject attackEffectPrefab; // 공격 이펙트 프리팹

    private float nextAttackTime = 0f; // 다음 공격 가능 시간
    private CharacterStats myStats;    // 내 스탯 참조

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
    }

    // AI나 Input 스크립트에서 공격 가능 여부를 확인할 때 사용
    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    // 1. 공격 트리거/쿨타임 시작 함수 (키 입력 또는 AI 공격 지시 시 호출)
    public void DoAttack()
    {
        if (!CanAttack()) return;

        // 쿨타임 갱신
        nextAttackTime = Time.time + attackCooldown;
    }

    // 2. 실제 타격 판정 및 이펙트 생성 함수 (애니메이션 이벤트에서 호출!)
    public void HitCheck()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning($"{gameObject.name}: attackPoint가 지정되지 않았습니다!");
            return;
        }

        // 이펙트 생성
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);
        }

        // 범위 내 타겟 감지
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
        
        // myStats에 저장된 공격력을 가져오고, 없으면 기본값 10 사용
        int damage = (myStats != null) ? myStats.AttackDamage : 10;

        foreach (Collider2D target in hitTargets)
        {
            // 나 자신을 때리는 경우 방지
            if (target.gameObject == gameObject) continue;

            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats != null)
            {
                targetStats.TakeDamage(damage);
                Debug.Log($"{gameObject.name}이(가) {target.name}에게 {damage} 데미지를 입혔습니다!");
            }
        }
    }

    // Scene 창에서 공격 범위를 시각적으로 확인
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}