using System.Collections.Generic;
using System;
using UnityEngine;

// 슬롯에 저장되는 아이템 데이터와 수량 구조
[System.Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int quantity;

    public InventorySlotData(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class InventoryUI : MonoBehaviour
{
    [Header("UI 연동")]
    [SerializeField] private GameObject slotPrefab;      // ItemSlot 프리팹
    [SerializeField] private Transform slotContainer;   // SlotContainer (Grid Layout)

    [Header("인벤토리 설정")]
    [SerializeField] private int totalSlotCount = 20;    // 총 슬롯 개수

    [Header("테스트 아이템 설정")]
    [SerializeField] private ItemData testItem1;        // 테스트 획득용 포션 (숫자 1)
    [SerializeField] private ItemData quickUseConsumable;
    [SerializeField] private KeyCode quickUseKey = KeyCode.Q;

    private List<InventorySlotData> inventoryDataList = new List<InventorySlotData>();
    private List<ItemSlot> slotUIList = new List<ItemSlot>();

    void Awake()
    {
        InitializeInventoryData();
        GenerateSlotsUI();
        if (quickUseConsumable == null) quickUseConsumable = testItem1;
        PotionQuickSlotUI.Create(this, quickUseConsumable, quickUseKey);
    }

    public event Action<ItemData, int> OnItemCountChanged;

    void Update()
    {
        // 테스트용: 숫자 1키로 포션 1개 획득
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testItem1 != null) AddItem(testItem1, 1);
        }

    }

    // 인벤토리 슬롯 데이터 초기화
    private void InitializeInventoryData()
    {
        inventoryDataList.Clear();
        for (int i = 0; i < totalSlotCount; i++)
        {
            inventoryDataList.Add(new InventorySlotData(null, 0));
        }
    }

    // 슬롯 UI 생성
    private void GenerateSlotsUI()
    {
        if (slotPrefab == null || slotContainer == null) return;

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        for (int i = 0; i < totalSlotCount; i++)
        {
            GameObject newSlotObj = Instantiate(slotPrefab, slotContainer);
            ItemSlot slot = newSlotObj.GetComponent<ItemSlot>();

            if (slot != null)
            {
                slot.ClearSlot();
                slotUIList.Add(slot);
            }
        }
    }

    // 🌟 핵심 기능: 아이템 획득 함수
    public bool AddItem(ItemData itemToAdd, int amount = 1)
    {
        if (itemToAdd == null || amount <= 0) return false;

        int availableCapacity = 0;
        if (itemToAdd.isStackable)
        {
            foreach (InventorySlotData slot in inventoryDataList)
            {
                if (slot.item == itemToAdd)
                    availableCapacity += Mathf.Max(0, itemToAdd.maxStackSize - slot.quantity);
                else if (slot.item == null)
                    availableCapacity += Mathf.Max(1, itemToAdd.maxStackSize);
            }
        }
        else
        {
            foreach (InventorySlotData slot in inventoryDataList)
                if (slot.item == null) availableCapacity++;
        }

        if (availableCapacity < amount)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        if (itemToAdd.isStackable)
        {
            for (int i = 0; i < inventoryDataList.Count; i++)
            {
                InventorySlotData slot = inventoryDataList[i];
                if (slot.item == itemToAdd && slot.quantity < itemToAdd.maxStackSize)
                {
                    int moved = Mathf.Min(amount, itemToAdd.maxStackSize - slot.quantity);
                    slot.quantity += moved;
                    amount -= moved;
                    if (amount == 0) break;
                }
            }
        }

        while (amount > 0)
        {
            int emptyIndex = inventoryDataList.FindIndex(slot => slot.item == null);
            if (emptyIndex < 0) return false; // capacity 검사로 도달하지 않아야 합니다.

            int stackAmount = itemToAdd.isStackable ? Mathf.Min(amount, Mathf.Max(1, itemToAdd.maxStackSize)) : 1;
            inventoryDataList[emptyIndex].item = itemToAdd;
            inventoryDataList[emptyIndex].quantity = stackAmount;
            amount -= stackAmount;
        }

        RefreshUI();
        Debug.Log($"[아이템 획득] {itemToAdd.itemName} 획득");
        return true;
    }

    // 데이터 변경 사항을 UI에 반영
    public void RefreshUI()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (inventoryDataList[i].item != null)
            {
                slotUIList[i].SetupSlot(inventoryDataList[i].item, inventoryDataList[i].quantity);
            }
            else
            {
                slotUIList[i].ClearSlot();
            }
        }
        if (quickUseConsumable != null)
            OnItemCountChanged?.Invoke(quickUseConsumable, GetItemCount(quickUseConsumable));
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (InventorySlotData slot in inventoryDataList)
            if (slot.item == item) total += slot.quantity;
        return total;
    }

    public bool UseQuickItem()
    {
        if (quickUseConsumable == null) return false;
        for (int i = 0; i < inventoryDataList.Count; i++)
            if (inventoryDataList[i].item == quickUseConsumable) return UseItem(i);
        return false;
    }
    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryDataList.Count) return false;

        InventorySlotData slotData = inventoryDataList[slotIndex];
        if (slotData.item == null) return false;

        if (slotData.item.itemType != ItemType.Consumable) return false;
        CharacterStats stats = FindPlayerStats();
        if (stats == null || !stats.Heal(slotData.item.healAmount)) return false;

        Debug.Log($"[아이템 사용] {slotData.item.itemName} 사용");
        slotData.quantity--;
        if (slotData.quantity <= 0)
        {
            slotData.item = null;
            slotData.quantity = 0;
        }
        RefreshUI();
        return true;
    }

    private CharacterStats FindPlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.GetComponentInParent<CharacterStats>() : null;
    }

    // 슬롯 인덱스 찾기 함수 (ItemSlot에서 자신 위치를 파악할 때 사용)
    public int GetSlotIndex(ItemSlot slot)
    {
        return slotUIList.IndexOf(slot);
    }
}
