using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ✨ TextMeshPro를 쓰기 위해 반드시 필요합니다!

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;    // 위로 올라가는 속도
    [SerializeField] private float fadeSpeed = 3.0f;    // 사라지는 속도
    [SerializeField] private float destroyTime = 0.5f;  // 생존 시간

    private TextMeshPro textMesh;
    private Color textColor;

    void Awake()
    {
        // 오브젝트에 붙어있는 TextMeshPro 컴포넌트를 가져옵니다.
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
    }

    void Start()
    {
        // 지정된 시간 뒤에 자동으로 메모리에서 삭제
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 1. 매 프레임마다 Y축(위쪽)으로 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 2. 텍스트를 점점 투명하게 만들기 (Alpha 값 감소)
        if (textMesh != null)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }
    }

    // ✨ 외부(CharacterStats)에서 데미지 숫자를 넘겨받아 세팅하는 함수
    public void SetDamage(int damageAmount)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damageAmount.ToString();
    }
}