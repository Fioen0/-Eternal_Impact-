using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows the active boss name and health at the top of the screen.</summary>
public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset bossNameFont;

    private GameObject panelRoot;
    private TextMeshProUGUI bossNameText;
    private Image healthFill;
    private CharacterStats currentBoss;

    private void Awake()
    {
        BuildHud();
        panelRoot.SetActive(false);
    }

    public void ShowBoss(CharacterStats boss, string displayName)
    {
        if (boss == null)
            return;

        UnsubscribeFromCurrentBoss();
        currentBoss = boss;
        currentBoss.OnHealthChanged.AddListener(UpdateHealth);
        currentBoss.Died += OnBossDied;

        bossNameText.text = string.IsNullOrWhiteSpace(displayName) ? boss.gameObject.name : displayName;
        UpdateHealth(currentBoss.CurrentHealth, currentBoss.MaxHealth);
        panelRoot.SetActive(true);
    }

    public void HideBoss()
    {
        UnsubscribeFromCurrentBoss();
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        UnsubscribeFromCurrentBoss();
    }

    private void OnBossDied()
    {
        // RoomManager hides the display once the boss death animation finishes and the room clears.
        UpdateHealth(0, currentBoss != null ? currentBoss.MaxHealth : 1);
    }

    private void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthFill != null)
            healthFill.fillAmount = maxHealth > 0 ? Mathf.Clamp01((float)currentHealth / maxHealth) : 0f;
    }

    private void UnsubscribeFromCurrentBoss()
    {
        if (currentBoss == null)
            return;

        currentBoss.OnHealthChanged.RemoveListener(UpdateHealth);
        currentBoss.Died -= OnBossDied;
        currentBoss = null;
    }

    private void BuildHud()
    {
        GameObject canvasObject = new GameObject(
            "BossHealthCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image panel = CreateImage("BossPanel", canvasObject.transform, new Color(0.035f, 0.025f, 0.035f, 0.9f));
        panelRoot = panel.gameObject;
        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -24f);
        panelRect.sizeDelta = new Vector2(660f, 104f);

        Image accent = CreateImage("TopAccent", panelRoot.transform, new Color(0.73f, 0.13f, 0.18f, 1f));
        RectTransform accentRect = accent.rectTransform;
        accentRect.anchorMin = new Vector2(0f, 1f);
        accentRect.anchorMax = new Vector2(1f, 1f);
        accentRect.pivot = new Vector2(0.5f, 1f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(0f, 3f);

        bossNameText = CreateText("BossName", panelRoot.transform);
        bossNameText.font = bossNameFont;
        bossNameText.fontSize = 26f;
        bossNameText.fontSizeMin = 18f;
        bossNameText.fontSizeMax = 28f;
        bossNameText.enableAutoSizing = true;
        bossNameText.alignment = TextAlignmentOptions.Center;
        bossNameText.color = new Color(1f, 0.93f, 0.82f, 1f);
        RectTransform nameRect = bossNameText.rectTransform;
        nameRect.anchorMin = new Vector2(0f, 1f);
        nameRect.anchorMax = new Vector2(1f, 1f);
        nameRect.pivot = new Vector2(0.5f, 1f);
        nameRect.anchoredPosition = new Vector2(0f, -10f);
        nameRect.sizeDelta = new Vector2(-28f, 40f);

        Image healthBackground = CreateImage("HealthBackground", panelRoot.transform,
            new Color(0.16f, 0.12f, 0.13f, 1f));
        RectTransform healthRect = healthBackground.rectTransform;
        healthRect.anchorMin = new Vector2(0.5f, 1f);
        healthRect.anchorMax = new Vector2(0.5f, 1f);
        healthRect.pivot = new Vector2(0.5f, 1f);
        healthRect.anchoredPosition = new Vector2(0f, -62f);
        healthRect.sizeDelta = new Vector2(610f, 20f);

        Image fill = CreateImage("HealthFill", healthBackground.transform,
            new Color(0.78f, 0.12f, 0.19f, 1f));
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;
        healthFill = fill;
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);
    }

    private static Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        return text;
    }
}
