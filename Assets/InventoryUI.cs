using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Inventory data and its runtime-generated slot UI.</summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField, Min(1)] private int totalSlotCount = 20;
    [SerializeField, Min(1)] private int columnCount = 5;
    [SerializeField] private CharacterStats characterStats;

    private readonly List<InventorySlotData> slots = new List<InventorySlotData>();
    private readonly List<ItemSlot> slotViews = new List<ItemSlot>();
    private ItemTooltipUI tooltip;

    private void Awake()
    {
        if (characterStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                characterStats = player.GetComponent<CharacterStats>();
        }

        BuildSlotViews();
        BuildTooltip();
        RefreshUI();
    }

    private void BuildSlotViews()
    {
        Transform container = transform.Find("InventorySlots");
        if (container == null)
        {
            GameObject grid = new GameObject("InventorySlots", typeof(RectTransform), typeof(GridLayoutGroup));
            container = grid.transform;
            container.SetParent(transform, false);
            RectTransform rect = (RectTransform)container;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(18f, 76f);
            rect.offsetMax = new Vector2(-18f, -18f);
        }

        GridLayoutGroup layout = container.GetComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(68f, 68f);
        layout.spacing = new Vector2(8f, 8f);
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = columnCount;

        slots.Clear();
        slotViews.Clear();
        for (int i = 0; i < totalSlotCount; i++)
        {
            slots.Add(new InventorySlotData());
            GameObject slotObject = new GameObject($"ItemSlot_{i + 1:00}", typeof(RectTransform), typeof(Image));
            slotObject.transform.SetParent(container, false);
            ItemSlot view = slotObject.AddComponent<ItemSlot>();
            view.Initialize(this, i, null);
            slotViews.Add(view);
        }
    }

    private void BuildTooltip()
    {
        GameObject tooltipObject = new GameObject("ItemTooltip", typeof(RectTransform), typeof(Image));
        tooltipObject.transform.SetParent(transform, false);
        RectTransform rect = (RectTransform)tooltipObject.transform;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 12f);
        rect.sizeDelta = new Vector2(-36f, 54f);

        tooltip = tooltipObject.AddComponent<ItemTooltipUI>();
        tooltip.Initialize(tooltipObject.GetComponent<Image>());
        tooltipObject.SetActive(false);
        foreach (ItemSlot slot in slotViews)
            slot.SetTooltip(tooltip);
    }

    /// <summary>Adds an item, filling existing stacks before using empty slots.</summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;
        int stackLimit = Mathf.Max(1, item.maxStackSize);

        if (item.isStackable)
        {
            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                InventorySlotData slot = slots[i];
                if (!SameItem(slot.item, item) || slot.amount >= stackLimit)
                    continue;

                int added = Mathf.Min(remaining, stackLimit - slot.amount);
                slot.amount += added;
                remaining -= added;
            }
        }

        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i].item != null)
                continue;

            int added = item.isStackable ? Mathf.Min(remaining, stackLimit) : 1;
            slots[i].item = item;
            slots[i].amount = added;
            remaining -= added;
        }

        RefreshUI();
        if (remaining > 0)
            Debug.LogWarning($"인벤토리가 가득 찼습니다. {item.itemName} {remaining}개를 추가하지 못했습니다.", this);

        return remaining == 0;
    }

    /// <summary>Uses a consumable and removes one only when its effect is applied.</summary>
    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return false;

        InventorySlotData slot = slots[slotIndex];
        if (slot.item == null || slot.item.itemType != ItemType.Consumable || slot.amount <= 0)
            return false;

        if (characterStats == null || !characterStats.Heal(slot.item.healAmount))
            return false;

        slot.amount--;
        if (slot.amount <= 0)
        {
            slot.item = null;
            slot.amount = 0;
            tooltip.Hide();
        }

        RefreshUI();
        return true;
    }

    public ItemData GetItem(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slots.Count ? slots[slotIndex].item : null;
    }

    public int GetAmount(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slots.Count ? slots[slotIndex].amount : 0;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
                slotViews[i].ClearSlot();
            else
                slotViews[i].SetupSlot(slots[i].item, slots[i].amount);
        }
    }

    private static bool SameItem(ItemData a, ItemData b)
    {
        if (a == b)
            return true;
        return a != null && b != null && !string.IsNullOrEmpty(a.itemId) && a.itemId == b.itemId;
    }

    private sealed class InventorySlotData
    {
        public ItemData item;
        public int amount;
    }
}
