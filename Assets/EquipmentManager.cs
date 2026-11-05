using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("유물 장착 슬롯")]
    public ItemData[] relicSlots = new ItemData[3];

    // 성유물 장착 (0~2번 슬롯)
    public bool EquipRelic(ItemData newRelic, int slotIndex)
    {
        if (newRelic == null || newRelic.itemType != ItemType.Relic) return false;
        if (slotIndex < 0 || slotIndex >= relicSlots.Length) return false;

        relicSlots[slotIndex] = newRelic;
        Debug.Log($"성유물 슬롯[{slotIndex}] 장착 완료: {newRelic.itemName}");
        return true;
    }
}
