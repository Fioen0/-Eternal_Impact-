using UnityEngine;

public enum ItemType { Weapon, Relic }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Header("스탯 증가량")]
    public int bonusAttackDamage;
    public int bonusMaxHealth;

    [TextArea]
    public string description;
}