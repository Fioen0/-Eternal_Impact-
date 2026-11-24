using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillQuickSlotUI : MonoBehaviour
{
    private static SkillQuickSlotUI instance;
    private static Sprite fillSprite;

    private PlayerSkillController skillController;
    private Image parryCooldownOverlay;
    private TextMeshProUGUI parryCooldownText;
    private Outline parryOutline;

    public static SkillQuickSlotUI Create(PlayerSkillController controller)
    {
        if (controller == null) return null;
        if (instance != null)
        {
            instance.skillController = controller;
            return instance;
        }

        GameObject root = new GameObject("SkillQuickSlotsHUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        instance = root.AddComponent<SkillQuickSlotUI>();
        instance.Initialize(controller);
        return instance;
    }

    private void Initialize(PlayerSkillController controller)
    {
        skillController = controller;

        CreateSlot("Parry", new Vector2(220, 32), "C", "P", "PARRY", true);
        CreateSlot("Wind", new Vector2(304, 32), "", "?", "WIND", false);
        CreateSlot("Ultimate", new Vector2(388, 32), "", "?", "ULT", false);
    }

    private void CreateSlot(string slotName, Vector2 position, string keyLabel, string iconLabel, string skillLabel, bool isReady)
    {
        GameObject slot = new GameObject(slotName + "Slot", typeof(RectTransform), typeof(Image), typeof(Outline));
        slot.layer = 5;
        slot.transform.SetParent(transform, false);

        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.anchorMin = Vector2.zero;
        slotRect.anchorMax = Vector2.zero;
        slotRect.pivot = Vector2.zero;
        slotRect.anchoredPosition = position;
        slotRect.sizeDelta = new Vector2(76, 76);

        Image background = slot.GetComponent<Image>();
        background.color = isReady
            ? new Color(0.045f, 0.04f, 0.035f, 0.9f)
            : new Color(0.035f, 0.04f, 0.05f, 0.78f);
        background.raycastTarget = false;

        Outline outline = slot.GetComponent<Outline>();
        outline.effectDistance = new Vector2(2, -2);
        outline.effectColor = isReady
            ? new Color(0.78f, 0.61f, 0.31f, 0.95f)
            : new Color(0.34f, 0.38f, 0.42f, 0.65f);

        if (isReady) parryOutline = outline;

        if (isReady)
        {
            parryCooldownOverlay = CreateImage("CooldownOverlay", slot.transform, new Color(0.01f, 0.01f, 0.015f, 0.78f));
            parryCooldownOverlay.sprite = GetFillSprite();
            parryCooldownOverlay.type = Image.Type.Filled;
            parryCooldownOverlay.fillMethod = Image.FillMethod.Vertical;
            parryCooldownOverlay.fillOrigin = (int)Image.OriginVertical.Bottom;
            parryCooldownOverlay.fillAmount = 0f;
            parryCooldownOverlay.enabled = false;
        }
        else
        {
            Image disabledTint = CreateImage("UnavailableTint", slot.transform, new Color(0.01f, 0.015f, 0.025f, 0.24f));
            Stretch(disabledTint.rectTransform);
        }

        if (!string.IsNullOrEmpty(keyLabel))
        {
            TextMeshProUGUI key = CreateText(slot.transform, "Key", keyLabel, 15, new Color(0.93f, 0.78f, 0.48f));
            Place(key.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(24, 20), new Vector2(8, -5));
            key.fontStyle = FontStyles.Bold;
        }

        TextMeshProUGUI icon = CreateText(slot.transform, "SkillIcon", iconLabel, isReady ? 32 : 27, isReady ? Color.white : new Color(0.58f, 0.62f, 0.66f));
        Place(icon.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(64, 44), new Vector2(0, 5));

        TextMeshProUGUI label = CreateText(slot.transform, "SkillName", skillLabel, 11, isReady ? new Color(0.93f, 0.86f, 0.72f) : new Color(0.55f, 0.59f, 0.63f));
        Place(label.rectTransform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(70, 20), new Vector2(0, 4));

        if (isReady)
        {
            parryCooldownText = CreateText(slot.transform, "CooldownTime", "", 21, Color.white);
            Place(parryCooldownText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(64, 34), Vector2.zero);
            parryCooldownText.fontStyle = FontStyles.Bold;
        }
    }

    private void Update()
    {
        if (skillController == null)
        {
            Destroy(gameObject);
            return;
        }

        if (parryCooldownOverlay == null) return;

        float remaining = skillController.ParryCooldownRemaining;
        parryCooldownOverlay.fillAmount = skillController.ParryCooldownFraction;
        parryCooldownOverlay.enabled = remaining > 0f;
        parryCooldownText.text = remaining > 0f ? Mathf.CeilToInt(remaining).ToString() : "";

        if (parryOutline != null)
        {
            parryOutline.effectColor = skillController.IsParrying
                ? new Color(1f, 0.84f, 0.42f, 1f)
                : new Color(0.78f, 0.61f, 0.31f, 0.95f);
        }
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private static Sprite GetFillSprite()
    {
        if (fillSprite == null)
            fillSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return fillSprite;
    }

    private static Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.layer = 5;
        obj.transform.SetParent(parent, false);
        Image image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        Stretch(image.rectTransform);
        return image;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string objectName, string value, float fontSize, Color color)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        obj.layer = 5;
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.raycastTarget = false;
        text.font = TMP_Settings.defaultFontAsset;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Place(RectTransform rect, Vector2 min, Vector2 max, Vector2 size, Vector2 offset)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;
    }
}
