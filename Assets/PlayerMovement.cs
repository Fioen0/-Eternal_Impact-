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
    [SerializeField] private float dashSpeed = 15f;    
    [SerializeField] private float dashTime = 0.2f;     
    [SerializeField] private float dashCooldown = 1f;   
    private bool isDashing = false;                     
    private bool canDash = true;                        

    private Rigidbody2D rb;
    private Animator anim;                              
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true; 
    private bool isAttacking = false;
    private CharacterStats myStats; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // ✨ 자식 오브젝트(Visual)의 Animator를 가져옵니다.
        anim = GetComponentInChildren<Animator>();               
        myStats = GetComponent<CharacterStats>();
    }
    
    public void SetHorizontalInput(float input)
    {
        if (isDashing) return; 
        horizontalInput = input;
    }
    
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        // isMoving 파라미터 업데이트
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f && !isDashing;
        if (anim != null)
        {
            anim.SetBool("isMoving", isMoving);
        }

        if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, groundLayer);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }

    public void SetAttacking(bool state)
    {
        isAttacking = state;
    }
    
    public void TryDash()
    {
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

        float dashDirection = horizontalInput != 0 ? horizontalInput : (transform.localScale.x >= 0 ? 1f : -1f);

        if (myStats != null) myStats.isInvincible = true;

        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        if (myStats != null) myStats.isInvincible = false;
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}