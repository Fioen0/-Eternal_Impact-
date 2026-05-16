using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float jumpForce = 12.0f;

    [Header("바닥 체크 설정")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true; // 현재 오른쪽을 보고 있는지 여부
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 좌우 입력 받기
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // ✨ [추가] 방향 뒤집기 체크
        // 왼쪽 키를 눌렀는데 오른쪽을 보고 있거나, 오른쪽 키를 눌렀는데 왼쪽을 보고 있다면!
        if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }

        // 2. 바닥에 닿아있는지 확인
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, groundLayer);

        // 3. 점프 입력 받기
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    // ✨ [추가] 실제 캐릭터의 스케일을 뒤집는 함수 (Update 괄호 밖에 적어주세요)
    void Flip()
    {
        // 상태를 반대로 스위치 (true -> false / false -> true)
        isFacingRight = !isFacingRight;

        // 현재 오브젝트의 Scale 값을 가져옵니다.
        Vector3 localScale = transform.localScale;
    
        // X축 Scale 값에 -1을 곱해서 부호를 반대로 뒤집습니다 (1 -> -1 / -1 -> 1)
        localScale.x *= -1f;
    
        // 뒤집은 값을 캐릭터에게 다시 적용합니다.
        transform.localScale = localScale;
    }

    void FixedUpdate()
    {
        // ✨ [수정] 만약 공격 중이라면 좌우 속도를 0으로 만들어 움직임을 완전히 멈춥니다!
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            // 기존 물리 이동 처리
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }
    // ✨ [추가] 외부에서 공격 상태를 켜고 끌 수 있는 함수 (Update 괄호 밖에 적어주세요)
    public void SetAttacking(bool state)
    {
        isAttacking = state;
    }
}
