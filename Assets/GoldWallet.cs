using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Session wallet. The balance survives room and scene changes.</summary>
public class GoldWallet : MonoBehaviour
{
    public static GoldWallet Instance { get; private set; }

    public int Balance { get; private set; }
    public event Action<int> BalanceChanged;
    private TMP_Text balanceText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateWallet()
    {
        if (Instance == null)
            new GameObject("GoldWallet").AddComponent<GoldWallet>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildHud();
        BalanceChanged += UpdateBalance;
        UpdateBalance(Balance);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        Balance = amount > int.MaxValue - Balance ? int.MaxValue : Balance + amount;
        BalanceChanged?.Invoke(Balance);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
            return false;

        Balance -= amount;
        BalanceChanged?.Invoke(Balance);
        return true;
    }
    /// <summary>Creates the top-right coin counter without requiring scene wiring.</summary>
    private void BuildHud()
    {
        GameObject canvasObject = new GameObject("GoldHUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = new GameObject("GoldPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-28f, -24f);
        panelRect.sizeDelta = new Vector2(210f, 64f);
        Image panelBackground = panel.GetComponent<Image>();
        panelBackground.color = new Color(0.055f, 0.045f, 0.035f, 0.78f);
        panelBackground.raycastTarget = false;

        GameObject iconObject = new GameObject("CoinIcon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(panel.transform, false);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(12f, 0f);
        iconRect.sizeDelta = new Vector2(40f, 40f);
        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = Resources.Load<Sprite>("gold_coin");
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        GameObject textObject = new GameObject("Balance", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(panel.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(62f, 4f);
        textRect.offsetMax = new Vector2(-14f, -4f);
        balanceText = textObject.GetComponent<TMP_Text>();
        balanceText.fontSize = 28f;
        balanceText.fontStyle = FontStyles.Bold;
        balanceText.color = new Color(1f, 0.91f, 0.62f, 1f);
        balanceText.alignment = TextAlignmentOptions.MidlineRight;
        balanceText.enableAutoSizing = true;
        balanceText.fontSizeMin = 16f;
        balanceText.fontSizeMax = 28f;
        balanceText.raycastTarget = false;
    }

    private void UpdateBalance(int balance)
    {
        if (balanceText != null)
            balanceText.text = balance.ToString("N0");
    }
}
