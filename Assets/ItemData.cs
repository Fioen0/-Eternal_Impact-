using UnityEngine;

// Keep the existing Weapon and Relic values first so saved assets remain compatible.
public enum ItemType { Weapon, Relic, Consumable, Material }
public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public ItemRarity rarity;

    [Header("중첩")]
    public bool isStackable;
    [Min(1)] public int maxStackSize = 99;

    [Header("소모품 효과")]
    [Min(0)] public int healAmount;

    [Header("스탯 증가량")]
    public int bonusAttackDamage;
    public int bonusMaxHealth;

    [TextArea]
    public string description;

    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon: return new Color(0.35f, 0.85f, 0.4f);
            case ItemRarity.Rare: return new Color(0.3f, 0.65f, 1f);
            case ItemRarity.Epic: return new Color(0.75f, 0.4f, 1f);
            case ItemRarity.Legendary: return new Color(1f, 0.65f, 0.2f);
            default: return Color.white;
        }
    }
}
