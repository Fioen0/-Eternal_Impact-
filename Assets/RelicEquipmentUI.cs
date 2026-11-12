using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows equipped relics in the existing relic tab.</summary>
public class RelicEquipmentUI : MonoBehaviour
{
    private Button[] slotButtons;
    private TMP_Text[] slotLabels;
    private EquipmentManager equipment;
    private TMP_FontAsset uiFont;

    public void Initialize(TMP_FontAsset font)
    {
        uiFont = font;
        BuildSlots();
        FindEquipment();
        Refresh();
    }

    private void OnEnable()
    {
        FindEquipment();
        Refresh();
    }

    private void OnDisable()
    {
        if (equipment != null) equipment.OnEquipmentChanged -= Refresh;
    }

    private void BuildSlots()
    {
        if (slotButtons != null) return;

        slotButtons = new Button[3];
        slotLabels = new TMP_Text[3];
        for (int i = 0; i < slotButtons.Length; i++)
        {
            GameObject slot = new GameObject($"RelicSlot_{i + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
            slot.transform.SetParent(transform, false);
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -22f - i * 92f);
            rect.sizeDelta = new Vector2(430f, 76f);

            Image background = slot.GetComponent<Image>();
            background.color = new Color(0.12f, 0.1f, 0.09f, 0.88f);
            Button button = slot.GetComponent<Button>();
            button.targetGraphic = background;
            int slotIndex = i;
            button.onClick.AddListener(() => TryUnequip(slotIndex));
            slotButtons[i] = button;

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(slot.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12f, 6f);
            labelRect.offsetMax = new Vector2(-12f, -6f);
            TMP_Text label = labelObject.GetComponent<TMP_Text>();
            label.fontSize = 20f;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 14f;
            label.fontSizeMax = 20f;
            label.raycastTarget = false;
            if (uiFont != null) label.font = uiFont;
            slotLabels[i] = label;
        }
    }

    private void FindEquipment()
    {
        EquipmentManager found = null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) found = player.GetComponent<EquipmentManager>();
        if (found == equipment) return;

        if (equipment != null) equipment.OnEquipmentChanged -= Refresh;
        equipment = found;
        if (equipment != null) equipment.OnEquipmentChanged += Refresh;
    }

    private void Refresh()
    {
        if (slotLabels == null) return;
        FindEquipment();
        for (int i = 0; i < slotLabels.Length; i++)
        {
            ItemData relic = equipment != null && i < equipment.relicSlots.Length ? equipment.relicSlots[i] : null;
            if (relic == null)
            {
                slotLabels[i].text = $"유물 슬롯 {i + 1}  ·  비어 있음";
                slotButtons[i].interactable = false;
            }
            else
            {
                string bonuses = string.Empty;
                if (relic.bonusMaxHealth != 0) bonuses += $"최대 체력 {(relic.bonusMaxHealth > 0 ? "+" : "")}{relic.bonusMaxHealth}  ";
                if (relic.bonusAttackDamage != 0) bonuses += $"공격력 {(relic.bonusAttackDamage > 0 ? "+" : "")}{relic.bonusAttackDamage}  ";
                slotLabels[i].text = $"{relic.itemName}\n{bonuses}클릭하여 해제";
                slotButtons[i].interactable = true;
            }
        }
    }

    private void TryUnequip(int slotIndex)
    {
        if (equipment == null || !equipment.UnequipRelicToInventory(slotIndex))
            Debug.Log("인벤토리가 가득 찼거나 해제할 유물이 없습니다.");
    }
}
