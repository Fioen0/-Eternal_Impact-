using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private CharacterAttack characterAttack;
    private PlayerMovement playerMovement; // ✨ [추가] 이동을 제어하기 위해 가져옵니다.
    
    [Header("공격 제어")]
    [SerializeField] private float stopDuration = 0.15f; // 이동이 멈출 시간 (이펙트 시간과 맞추면 좋아요)
    
    void Start()
    {
        // 같은 오브젝트(Player)에 붙어있는 CharacterAttack 컴포넌트를 가져옵니다.
        characterAttack = GetComponent<CharacterAttack>();
        playerMovement = GetComponent<PlayerMovement>(); // 컴포넌트 연결
    }

    void Update()
    {
        // 키보드 Z 키를 누르면 공격 명령을 보냅니다!
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (characterAttack != null)
            {
                // ✨ [수정] 그냥 공격하는 게 아니라, 이동을 멈추는 코루틴을 실행합니다!
                StartCoroutine(AttackRoutine());
            }
        }
        // ✨ 3. [추가] 대쉬 키 입력 (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("1. Left Shift 키 입력 감지됨!"); // 👈 이 메시지가 콘솔에 뜨는지 확인!
            if (playerMovement != null) 
            {
                playerMovement.TryDash();
            }
            else
            {
                Debug.LogError("movement 스크립트를 찾을 수 없습니다!");
            }
        }
    }
    // ✨ [추가] 공격하는 동안 이동을 멈추게 하는 핵심 로직
    IEnumerator AttackRoutine()
    {
        // 1. 이동 스크립트에게 공격 시작했다고 알림 (이동 멈춤)
        playerMovement.SetAttacking(true);

        // 2. 실제 공격 및 이펙트 발동!
        characterAttack.DoAttack();

        // 3. stopDuration(0.15초) 동안 코드 흐름을 잠시 대기시킵니다.
        yield return new WaitForSeconds(stopDuration);

        // 4. 대기가 끝나면 다시 움직일 수 있게 풀어줍니다.
        playerMovement.SetAttacking(false);
    }
}