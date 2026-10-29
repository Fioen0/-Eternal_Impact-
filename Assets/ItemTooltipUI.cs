using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Small runtime tooltip shown when an inventory item is left-clicked.</summary>
public class ItemTooltipUI : MonoBehaviour
{
    private TMP_Text label;

    public void Initialize(Image background)
    {
        background.color = new Color(0.06f, 0.06f, 0.08f, 0.96f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("Description", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(transform, false);
        RectTransform rect = (RectTransform)textObject.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10f, 5f);
        rect.offsetMax = new Vector2(-10f, -5f);
        label = textObject.GetComponent<TMP_Text>();
        label.fontSize = 14f;
        label.color = Color.white;
        label.raycastTarget = false;
        label.enableWordWrapping = true;
    }

    public void Show(ItemData item)
    {
        if (item == null || label == null)
            return;

        string description = string.IsNullOrWhiteSpace(item.description) ? "설명이 없습니다." : item.description;
        label.text = $"<b>{item.itemName}</b>  ({item.rarity})\n{description}";
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
