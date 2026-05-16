using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본 스텟 설정")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;
    [SerializeField] private int attack = 10;
    [SerializeField] private int defense = 5;
    [Header("시각 효과")]
    [SerializeField] private GameObject damagePopupPrefab; // 방금 만든 팝업 프리팹 등록창
    [SerializeField] private HealthBar healthBar;
    
    // 다른 스크립트에서 현재 체력이나 공격력을 읽을 수 있도록 프로퍼티 제공
    public int CurrentHp => currentHp;
    public int Attack => attack;
    public int Defense => defense;

    void Awake()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 초기화
        currentHp = maxHp;
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHp, maxHp);
        }
    }

    // ⚔️ 데미지를 계산하고 적용하는 핵심 함수
    public void TakeDamage(int incomingDamage)
    {
        // 최종 데미지 = 들어온 공격력 - 나의 방어력
        int finalDamage = incomingDamage - defense;

        // 방어력이 너무 높아 데미지가 0 이하가 되는 것을 방지 (최소 1 데미지 보장)
        finalDamage = Mathf.Max(finalDamage, 1);

        // 현재 체력 차감
        currentHp -= finalDamage;
        
        // 디버그 콘솔창에 로그 출력 (오브젝트 이름과 남은 체력 표시)
        Debug.Log($"{gameObject.name}이(가) {finalDamage}의 데미지를 입었습니다. 남은 HP: {currentHp}");
        
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHp, maxHp);
        }
        
        // ✨ [추가] 데미지 팝업 소환 로직
        if (damagePopupPrefab != null)
        {
            // 팝업이 겹치지 않게 머리 위쪽(Y축 + 1)에 약간의 랜덤 좌우 오프셋을 줘서 소환합니다.
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 1.0f, 0);
        
            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
        
            // 소환된 팝업의 스크립트를 찾아가서 데미지 숫자를 주입합니다.
            FloatingText floatingText = popup.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.SetDamage(finalDamage);
            }
        }
        
        // 체력이 0 이하가 되면 사망 처리
        if (currentHp <= 0)
        {
            Die();
        }
    }

    // 💀 사망 처리 함수
    private void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 사망했습니다.");
        
        // 로그라이크 특성에 맞게 플레이어와 몹의 사망 분기 처리 가능
        if (gameObject.CompareTag("Player"))
        {
            // TODO: 플레이어 게임오버 팝업 띄우기 등
        }
        else
        {
            // 몬스터인 경우 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}