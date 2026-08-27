using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("장착 슬롯")]
    public ItemData weaponSlot;
    public ItemData[] relicSlots = new ItemData[3]; // 성유물 슬롯 3개

    private CharacterStats playerStats;

    void Start()
    {
        playerStats = GetComponent<CharacterStats>();
    }

    // 무기 장착
    public void EquipWeapon(ItemData newWeapon)
    {
        if (newWeapon.itemType != ItemType.Weapon) return;
        weaponSlot = newWeapon;
        Debug.Log($"무기 장착 완료: {newWeapon.itemName}");
    }

    // 성유물 장착 (0~2번 슬롯)
    public void EquipRelic(ItemData newRelic, int slotIndex)
    {
        if (newRelic.itemType != ItemType.Relic) return;
        if (slotIndex < 0 || slotIndex >= relicSlots.Length) return;

        relicSlots[slotIndex] = newRelic;
        Debug.Log($"성유물 슬롯[{slotIndex}] 장착 완료: {newRelic.itemName}");
    }
}