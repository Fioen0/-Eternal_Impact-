using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private CharacterAttack characterAttack;
    private PlayerMovement playerMovement;
    private Animator anim; 

    [Header("공격 제어")]
    [SerializeField] private float stopDuration = 0.15f;
    
    void Start()
    {
        characterAttack = GetComponent<CharacterAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        // ✨ 자식 오브젝트(Visual)의 Animator를 가져옵니다.
        anim = GetComponentInChildren<Animator>(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (characterAttack != null)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (playerMovement != null) 
            {
                playerMovement.TryDash();
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        if (playerMovement != null) playerMovement.SetAttacking(true);

        if (characterAttack != null) characterAttack.DoAttack();

        yield return new WaitForSeconds(stopDuration);

        if (playerMovement != null) playerMovement.SetAttacking(false);
    }
}