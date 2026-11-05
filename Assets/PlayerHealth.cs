using System;
using UnityEngine;

/// <summary>Compatibility adapter for older callers; CharacterStats owns health.</summary>
public class PlayerHealth : MonoBehaviour
{
    private CharacterStats stats;
    public int CurrentHealth => stats != null ? stats.CurrentHealth : 0;
    public int MaxHealth => stats != null ? stats.MaxHealth : 0;
    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
        if (stats == null)
            Debug.LogError("PlayerHealth requires CharacterStats on the same GameObject.", this);
    }

    private void OnEnable()
    {
        if (stats != null) stats.OnHealthChanged.AddListener(ForwardHealthChanged);
    }

    private void Start()
    {
        if (stats != null) ForwardHealthChanged(stats.CurrentHealth, stats.MaxHealth);
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnHealthChanged.RemoveListener(ForwardHealthChanged);
    }

    public void Heal(int amount)
    {
        if (stats != null) stats.Heal(amount);
    }

    public void TakeDamage(int damage)
    {
        if (stats != null) stats.TakeDamage(damage);
    }

    private void ForwardHealthChanged(int current, int maximum) => OnHealthChanged?.Invoke(current, maximum);
}
