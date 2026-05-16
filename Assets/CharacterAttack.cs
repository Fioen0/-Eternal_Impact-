using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private Transform attackPoint;   // 공격 중심점
    [SerializeField] private float attackRange = 0.5f;   // 공격 반지름 범위
    [SerializeField] private LayerMask targetLayer;      // 타겟 레이어 (내가 때릴 대상의 레이어)
    [SerializeField] private float attackCooldown = 0.3f; // 공격 쿨타임
    private float nextAttackTime = 0f;                   // 다음 공격 가능 시간
    
    [Header("이펙트 설정")]
    [SerializeField] private GameObject attackEffectPrefab; // 소환할 이펙트 프리팹

    private CharacterStats myStats; // 나의 스텟 컴포넌트

    void Start()
    {
        // 같은 오브젝트에 붙어있는 CharacterStats 컴포넌트를 가져옵니다.
        myStats = GetComponent<CharacterStats>();
    }
    
    // ⭐ 핵심: 외부(PlayerInput이나 MobAI)에서 이 함수를 호출해서 공격을 발동시킵니다!
    public void DoAttack()
    {
        // 쿨타임 체크
        if (Time.time < nextAttackTime) return;

        Debug.Log($"{gameObject.name}이(가) 범용 공격 시스템으로 공격합니다!");
        // ✨ [추가] 공격 이펙트 소환
        if (attackEffectPrefab != null && attackPoint != null)
        {
            // attackPoint 위치와 방향(rotation) 그대로 이펙트 오브젝트를 생성합니다.
            Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);
        }

        // 범위 내의 적들을 감지 (이제 enemyLayer 대신 targetLayer라는 이름을 씁니다)
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);

        // 타겟들에게 데미지 전달
        foreach (Collider2D target in hitTargets)
        {
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats != null)
            {
                // 나의 공격력 스텟을 타겟에게 전달
                targetStats.TakeDamage(myStats.Attack);
            }
        }

        // 쿨타임 갱신
        nextAttackTime = Time.time + attackCooldown;
    }

    // 에디터 화면에서 공격 범위를 시각적으로 표시
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
