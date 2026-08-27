using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Slider hpSlider;

    [Header("스탯 참조 (비워두면 부모 오브젝트에서 자동으로 찾습니다)")]
    [SerializeField] private CharacterStats targetStats;

    private void Awake()
    {
        if (hpSlider == null)
        {
            hpSlider = GetComponent<Slider>();
        }

        if (targetStats == null)
        {
            targetStats = GetComponentInParent<CharacterStats>();
        }
    }

    private void OnEnable()
    {
        if (targetStats != null)
        {
            targetStats.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    private void OnDisable()
    {
        if (targetStats != null)
        {
            targetStats.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    // int 매개변수(현재 체력, 최대 체력)를 받아서 비율 계산 처리
    public void UpdateHealthBar(int currentHp, int maxHp)
    {
        if (hpSlider != null && maxHp > 0)
        {
            hpSlider.value = (float)currentHp / maxHp;
            Debug.Log($"[{gameObject.name}] 체력바 갱신: {currentHp} / {maxHp}");
        }
    }
}