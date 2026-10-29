using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Visual view and pointer actions for one inventory slot.</summary>
public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    private Image iconImage;
    private TMP_Text amountText;
    private InventoryUI inventory;
    private ItemTooltipUI tooltip;
    private int slotIndex;
    private ItemData currentItem;

    public void Initialize(InventoryUI owner, int index, ItemTooltipUI itemTooltip)
    {
        inventory = owner;
        slotIndex = index;
        tooltip = itemTooltip;

        Image background = GetComponent<Image>();
        background.color = new Color(0.12f, 0.12f, 0.14f, 0.9f);
        background.raycastTarget = true;

        GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        icon.transform.SetParent(transform, false);
        RectTransform iconRect = (RectTransform)icon.transform;
        iconRect.anchorMin = new Vector2(0.08f, 0.08f);
        iconRect.anchorMax = new Vector2(0.92f, 0.92f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        iconImage = icon.GetComponent<Image>();
        iconImage.preserveAspect = true;

        // Outline is a UI effect; its color follows the item's rarity.
        gameObject.AddComponent<Outline>();
        Outline outline = GetComponent<Outline>();
        outline.effectDistance = new Vector2(2f, -2f);
        outline.enabled = false;

        GameObject count = new GameObject("Amount", typeof(RectTransform), typeof(TextMeshProUGUI));
        count.transform.SetParent(transform, false);
        RectTransform countRect = (RectTransform)count.transform;
        countRect.anchorMin = new Vector2(0.55f, 0f);
        countRect.anchorMax = new Vector2(1f, 0.35f);
        countRect.offsetMin = new Vector2(-2f, 2f);
        countRect.offsetMax = new Vector2(-4f, -2f);
        amountText = count.GetComponent<TMP_Text>();
        amountText.alignment = TextAlignmentOptions.BottomRight;
        amountText.fontSize = 16f;
        amountText.raycastTarget = false;
    }

    public void SetTooltip(ItemTooltipUI itemTooltip) => tooltip = itemTooltip;

    public void SetupSlot(ItemData item, int amount)
    {
        currentItem = item;
        iconImage.sprite = item != null ? item.icon : null;
        iconImage.enabled = item != null && item.icon != null;

        Outline outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = item != null ? item.GetRarityColor() : Color.clear;
            outline.enabled = item != null;
        }

        amountText.text = item != null && amount > 1 ? amount.ToString() : string.Empty;
        amountText.enabled = item != null && amount > 1;
    }

    public void ClearSlot()
    {
        currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = string.Empty;
        amountText.enabled = false;
        Outline outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            tooltip?.Show(currentItem);
        else if (eventData.button == PointerEventData.InputButton.Right)
            inventory?.UseItem(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip?.Hide();
    }
}
