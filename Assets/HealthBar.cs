using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ✨ UI(Slider)를 제어하기 위해 반드시 필요합니다!

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider; // 연결할 슬라이더 컴포넌트

    // ✨ 체력바의 최대 수치와 현재 수치를 세팅하는 함수
    public void UpdateHealthBar(float currentHp, float maxHp)
    {
        if (hpSlider != null)
        {
            // 슬라이더의 value는 0에서 1 사이의 비율값으로 작동하게 합니다.
            hpSlider.value = currentHp / maxHp;
        }
    }
}
