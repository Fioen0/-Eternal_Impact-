using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private CharacterAttack characterAttack;
    private PlayerMovement playerMovement;
    private Animator anim; 

    [Header("공격 제어")]
    [SerializeField] private float stopDuration = 1.0f; // Animation-event fallback in case the attack clip is interrupted.

    [Header("스킬 1 - 패링")]
    [SerializeField] private KeyCode parrySkillKey = KeyCode.C;
    [SerializeField, Min(0f)] private float parryDuration = 1.5f;
    [SerializeField, Min(0f)] private float parryCooldown = 5f;
    private CharacterStats characterStats;
    private PlayerSkillController skillController;
    private bool isAttacking;
    
    void Start()
    {
        characterAttack = GetComponent<CharacterAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        characterStats = GetComponent<CharacterStats>();
        skillController = GetComponent<PlayerSkillController>();
        if (skillController == null)
            skillController = gameObject.AddComponent<PlayerSkillController>();
        skillController.ConfigureParry(parryDuration, parryCooldown);

        // ✨ 자식 오브젝트(Visual)의 Animator를 가져옵니다.
        anim = GetComponentInChildren<Animator>(); 

        if (characterStats != null)
            characterStats.Died += FinishAttack;
    }

    private void OnDestroy()
    {
        if (characterStats != null)
            characterStats.Died -= FinishAttack;
    }

    void Update()
    {
        if (ShopUI.IsOpen || (characterStats != null && characterStats.isDead))
            return;

        if (Input.GetKeyDown(parrySkillKey) && skillController != null)
            skillController.TryUseParry();

        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking && (playerMovement == null || playerMovement.CanAttack))
        {
            if (characterAttack != null && characterAttack.CanAttack())
            {
                StartCoroutine(AttackRoutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking)
        {
            if (playerMovement != null) 
            {
                playerMovement.TryDash();
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        if (isAttacking || (characterStats != null && characterStats.isDead))
            yield break;

        isAttacking = true;
        if (playerMovement != null) playerMovement.SetAttacking(true);
        if (characterAttack != null) characterAttack.DoAttack();

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        float elapsed = 0f;
        while (isAttacking && elapsed < stopDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        FinishAttack();
    }

    // Called by the final animation event on the attack clip.
    public void OnAttackAnimationFinished()
    {
        FinishAttack();
    }

    private void FinishAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;
        if (playerMovement != null)
            playerMovement.SetAttacking(false);
    }
}
