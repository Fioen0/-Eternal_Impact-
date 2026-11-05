using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Screen-space quick slot for the selected consumable.</summary>
public class PotionQuickSlotUI : MonoBehaviour
{
    private InventoryUI inventory;
    private ItemData item;
    private KeyCode useKey;
    private Image iconImage;
    private TextMeshProUGUI quantityText;

    public static PotionQuickSlotUI Create(InventoryUI inventory, ItemData item, KeyCode useKey)
    {
        if (inventory == null || item == null) return null;

        GameObject root = new GameObject("PotionQuickSlotHUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        PotionQuickSlotUI quickSlot = root.AddComponent<PotionQuickSlotUI>();
        quickSlot.Initialize(inventory, item, useKey);
        return quickSlot;
    }

    private void Initialize(InventoryUI sourceInventory, ItemData quickItem, KeyCode key)
    {
        inventory = sourceInventory;
        item = quickItem;
        useKey = key;

        RectTransform panel = CreateRect("Panel", transform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(32, 32), new Vector2(174, 82), new Vector2(0, 0));
        Image background = panel.gameObject.AddComponent<Image>();
        background.color = new Color(0.035f, 0.03f, 0.025f, 0.78f);
        background.raycastTarget = false;
        Outline outline = panel.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.72f, 0.58f, 0.32f, 0.82f);
        outline.effectDistance = new Vector2(2, -2);

        RectTransform iconRect = CreateRect("Potion Icon", panel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(18, 0), new Vector2(52, 52), new Vector2(0, 0.5f));
        iconImage = iconRect.gameObject.AddComponent<Image>();
        iconImage.sprite = item.icon;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;

        RectTransform countRect = CreateRect("Potion Count", panel, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(76, -2), new Vector2(-112, 38), new Vector2(0, 0.5f));
        quantityText = CreateText(countRect, "x0", 30, TextAlignmentOptions.MidlineLeft, Color.white);

        RectTransform keyRect = CreateRect("Shortcut Key", panel, new Vector2(1, 0), new Vector2(1, 0), new Vector2(-10, 8), new Vector2(38, 26), new Vector2(1, 0));
        TextMeshProUGUI keyText = CreateText(keyRect, useKey.ToString(), 18, TextAlignmentOptions.BottomRight, new Color(0.92f, 0.77f, 0.45f));
        keyText.fontStyle = FontStyles.Bold;

        inventory.OnItemCountChanged += OnItemCountChanged;
        RefreshCount(inventory.GetItemCount(item));
    }

    private void Update()
    {
        if (Input.GetKeyDown(useKey)) inventory.UseQuickItem();
    }

    private void OnItemCountChanged(ItemData changedItem, int count)
    {
        if (changedItem == item) RefreshCount(count);
    }

    private void RefreshCount(int count)
    {
        quantityText.text = "x" + count;
        quantityText.color = count > 0 ? Color.white : new Color(0.72f, 0.68f, 0.62f);
        iconImage.color = count > 0 ? Color.white : new Color(0.55f, 0.55f, 0.55f, 0.75f);
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.OnItemCountChanged -= OnItemCountChanged;
    }

    private static RectTransform CreateRect(string objectName, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Vector2 pivot)
    {
        GameObject child = new GameObject(objectName, typeof(RectTransform));
        child.layer = 5;
        child.transform.SetParent(parent, false);
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.pivot = pivot;
        return rect;
    }

    private static TextMeshProUGUI CreateText(RectTransform rect, string value, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.font = TMP_Settings.defaultFontAsset;
        return text;
    }
}
