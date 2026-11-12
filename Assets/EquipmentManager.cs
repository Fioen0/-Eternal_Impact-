using System;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class EquipmentManager : MonoBehaviour
{
    [Header("유물 장착 슬롯")]
    public ItemData[] relicSlots = new ItemData[3];

    private CharacterStats characterStats;
    public event Action OnEquipmentChanged;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        if (relicSlots == null || relicSlots.Length == 0)
            relicSlots = new ItemData[3];
    }

    private void Start() => RecalculateBonuses();

    public int FirstEmptySlotIndex
    {
        get
        {
            for (int i = 0; i < relicSlots.Length; i++)
                if (relicSlots[i] == null) return i;
            return -1;
        }
    }

    // 성유물 장착 (0~2번 슬롯)
    public bool EquipRelic(ItemData newRelic, int slotIndex)
    {
        if (newRelic == null || newRelic.itemType != ItemType.Relic) return false;
        if (slotIndex < 0 || slotIndex >= relicSlots.Length) return false;

        relicSlots[slotIndex] = newRelic;
        RecalculateBonuses();
        OnEquipmentChanged?.Invoke();
        Debug.Log($"성유물 슬롯[{slotIndex}] 장착 완료: {newRelic.itemName}");
        return true;
    }

    public bool UnequipRelic(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= relicSlots.Length || relicSlots[slotIndex] == null)
            return false;

        relicSlots[slotIndex] = null;
        RecalculateBonuses();
        OnEquipmentChanged?.Invoke();
        return true;
    }

    public bool UnequipRelicToInventory(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= relicSlots.Length || relicSlots[slotIndex] == null)
            return false;

        InventoryUI inventory = FindObjectOfType<InventoryUI>(true);
        ItemData relic = relicSlots[slotIndex];
        if (inventory == null || !inventory.CanAddItem(relic, 1))
            return false;

        if (!inventory.AddItem(relic, 1))
            return false;

        return UnequipRelic(slotIndex);
    }

    private void RecalculateBonuses()
    {
        if (characterStats == null) characterStats = GetComponent<CharacterStats>();

        int maxHealthBonus = 0;
        int attackDamageBonus = 0;
        foreach (ItemData relic in relicSlots)
        {
            if (relic == null || relic.itemType != ItemType.Relic) continue;
            maxHealthBonus += relic.bonusMaxHealth;
            attackDamageBonus += relic.bonusAttackDamage;
        }

        if (characterStats != null)
            characterStats.SetEquipmentBonuses(maxHealthBonus, attackDamageBonus);
    }
}
