using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시

public class ItemTooltipUI : MonoBehaviour
{
    [Header("UI 요소 참조")]
    [SerializeField] private TextMeshProUGUI itemNameText;     // 아이템 이름
    [SerializeField] private TextMeshProUGUI itemRarityText;   // 아이템 등급
    [SerializeField] private Image itemIconImage;              // 아이템 아이콘
    [SerializeField] private TextMeshProUGUI itemStatsText;    // 스탯 (공격력, 체력 등)
    [SerializeField] private TextMeshProUGUI itemDescText;     // 아이템 설명

    void Awake()
    {
        // 시작 시 툴팁 창 숨기기
        HideTooltip();
    }

    // 툴팁 창에 아이템 정보 세팅 및 표시
    public void ShowTooltip(ItemData item)
    {
        if (item == null) return;

        // 1. 이름 및 등급 설정
        itemNameText.text = item.itemName;
        itemNameText.color = item.GetRarityColor(); // 등급별 색상 적용

        itemRarityText.text = item.rarity.ToString();
        itemRarityText.color = item.GetRarityColor();

        // 2. 아이콘 설정
        if (itemIconImage != null)
        {
            itemIconImage.sprite = item.icon;
            itemIconImage.enabled = true;
        }

        // 3. 스탯 정보 텍스트 조합
        string statsStr = "";
        if (item.bonusAttackDamage > 0) statsStr += $"공격력 +{item.bonusAttackDamage}\n";
        if (item.bonusMaxHealth > 0) statsStr += $"최대 체력 +{item.bonusMaxHealth}\n";
        
        itemStatsText.text = statsStr;

        // 4. 설명 설정
        itemDescText.text = item.description;

        // 툴팁 패널 활성화
        gameObject.SetActive(true);
    }

    // 툴팁 창 숨기기
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}