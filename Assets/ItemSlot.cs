using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 이벤트 처리에 필수
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [Header("UI 요소 참조")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private ItemData currentItem;
    private int currentQuantity;

    // 슬롯 바인딩
    public void SetupSlot(ItemData item, int quantity = 1)
    {
        currentItem = item;
        currentQuantity = quantity;

        if (currentItem != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true;

            if (borderImage != null)
            {
                borderImage.color = currentItem.GetRarityColor();
                borderImage.enabled = true;
            }

            if (quantityText != null)
            {
                if (currentItem.isStackable && currentQuantity > 1)
                {
                    quantityText.text = currentQuantity.ToString();
                    quantityText.gameObject.SetActive(true);
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ClearSlot();
        }
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;

        if (iconImage != null) iconImage.enabled = false;
        if (borderImage != null) borderImage.enabled = false;
        if (quantityText != null) quantityText.gameObject.SetActive(false);
    }

    // 아이템 사용은 퀵 슬롯 단축키로 처리하고, 클릭은 툴팁 표시만 담당합니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemTooltipUI tooltip = FindObjectOfType<ItemTooltipUI>(true);
            if (tooltip != null)
            {
                tooltip.ShowTooltip(currentItem);
            }
        }
    }

    // 🌟 마우스 커서가 슬롯을 벗어났을 때 툴팁 숨기기
    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI tooltip = FindObjectOfType<ItemTooltipUI>(true);
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
}
