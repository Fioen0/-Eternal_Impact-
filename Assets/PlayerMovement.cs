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
    
    [Header("대쉬 설정")]
    [SerializeField] private float dashSpeed = 15f;    // 대쉬할 때의 순간 속도
    [SerializeField] private float dashTime = 0.2f;     // 대쉬 지속 시간 (무적 타임)
    [SerializeField] private float dashCooldown = 1f;   // 대쉬 쿨타임
    private bool isDashing = false;                     // 현재 대쉬 중인지 체크
    private bool canDash = true;                        // 대쉬 사용 가능 여부

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true; // 현재 오른쪽을 보고 있는지 여부
    private bool isAttacking = false;
    private CharacterStats myStats; // 내 스탯 컴포넌트 (무적 상태 조절용)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myStats = GetComponent<CharacterStats>();
    }
    
    // PlayerInput에서 방향값을 매 프레임 전달받는 함수
    public void SetHorizontalInput(float input)
    {
        if (isDashing) return; // 대쉬 중일 때는 이동 입력 무시
        horizontalInput = input;
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
        // ✨ [핵심] 대쉬 중일 때는 FixedUpdate가 속도를 건드리지 못하게 무조건 가로막아야 합니다!
        if (isDashing) return;

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
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
        
        // 대쉬 중일 때는 물리 이동 연산을 멈추고 대쉬 코루틴에 이동을 맡깁니다.
        if (isDashing) return;

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
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
    
    // ✨ [핵심] PlayerInput에서 Shift 키 누르면 이 함수를 호출합니다!
    public void TryDash()
    {
        Debug.Log($"2. TryDash 호출됨! canDash: {canDash}, isDashing: {isDashing}"); // 👈 신호가 넘어왔는지 확인!
        if (canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }
    
    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // ✨ [수정] scale.x 대신, 키보드를 누른 방향(horizontalInput)이 있으면 그쪽으로 대쉬!
        // 아무것도 안 누르고 있으면 캐릭터가 보고 있는 방향(scale.x)으로 대쉬!
        float dashDirection = horizontalInput != 0 ? horizontalInput : (transform.localScale.x >= 0 ? 1f : -1f);

        if (myStats != null) myStats.isInvincible = true;

        // 🚀 속도를 줄 때 velocity를 덮어씌웁니다.
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        if (myStats != null) myStats.isInvincible = false;
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero; // 대쉬 종료 후 멈춤
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}

