using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private CharacterAttack characterAttack;

    void Awake()
    {
        // 부모 오브젝트(mob)에 붙어있는 CharacterAttack 스크립트를 가져옵니다.
        characterAttack = GetComponentInParent<CharacterAttack>();

        if (characterAttack == null)
        {
            Debug.LogError($"{gameObject.name}: 부모 오브젝트에서 CharacterAttack 스크립트를 찾을 수 없습니다!");
        }
    }

    // ✨ Animation Event가 호출할 중계 함수
    public void HitCheck()
    {
        if (characterAttack != null)
        {
            // 부모의 CharacterAttack 스크립트에 있는 HitCheck() 함수를 호출합니다.
            characterAttack.HitCheck();
        }
    }
}