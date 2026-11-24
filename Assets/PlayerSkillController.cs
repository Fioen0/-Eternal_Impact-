using System.Collections;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    private CharacterStats characterStats;
    private float parryDuration;
    private float parryCooldown;
    private float nextParryTime;
    private Coroutine parryRoutine;

    public bool IsParrying { get; private set; }
    public float ParryCooldownRemaining => Mathf.Max(0f, nextParryTime - Time.time);
    public float ParryCooldownFraction => parryCooldown > 0f ? ParryCooldownRemaining / parryCooldown : 0f;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }

    public void ConfigureParry(float duration, float cooldown)
    {
        parryDuration = Mathf.Max(0f, duration);
        parryCooldown = Mathf.Max(parryDuration, cooldown);
        SkillQuickSlotUI.Create(this);
    }

    public bool TryUseParry()
    {
        if (IsParrying || Time.time < nextParryTime ||
            (characterStats != null && characterStats.isDead))
        {
            return false;
        }

        nextParryTime = Time.time + parryCooldown;
        if (parryRoutine != null) StopCoroutine(parryRoutine);
        parryRoutine = StartCoroutine(ParryRoutine());
        Debug.Log($"패링 발동! {parryDuration:0.0}초 동안 적 투사체를 막습니다.", this);
        return true;
    }

    private IEnumerator ParryRoutine()
    {
        IsParrying = true;
        yield return new WaitForSeconds(parryDuration);
        IsParrying = false;
        parryRoutine = null;
    }

    private void OnDisable()
    {
        if (parryRoutine != null)
        {
            StopCoroutine(parryRoutine);
            parryRoutine = null;
        }

        IsParrying = false;
    }
}
