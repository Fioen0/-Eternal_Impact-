using UnityEngine;

// 아이템 종류 정의
public enum ItemType
{
    // Explicit values preserve existing ScriptableObject data (Relic=1, Consumable=2).
    Relic = 1,
    Consumable = 2
}

// 아이템 등급 정의
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;      // Relic or Consumable
    public ItemRarity rarity;

    [Header("수량 관련")]
    public bool isStackable;       // 중첩 가능 여부
    public int maxStackSize = 99;  // 최대 중첩 수량

    [Header("스탯 및 효과")]
    public int bonusAttackDamage;  // 추가 공격력 (장비용)
    public int bonusMaxHealth;     // 추가 최대 체력 (장비용)
    public int healAmount;         // 🌟 [추가] 체력 회복량 (소모품용)

    [TextArea(3, 5)]
    public string description;     // 아이템 상세 설명

    // 등급별 색상 반환 함수
    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case ItemRarity.Common: return Color.white;
            case ItemRarity.Uncommon: return Color.green;
            case ItemRarity.Rare: return new Color(0.2f, 0.6f, 1f); // 파란색
            case ItemRarity.Epic: return new Color(0.6f, 0.2f, 1f); // 보라색
            case ItemRarity.Legendary: return new Color(1f, 0.5f, 0f); // 주황색
            default: return Color.white;
        }
    }
}
