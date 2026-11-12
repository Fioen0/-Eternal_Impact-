using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Shop window opened by the shop room interaction.</summary>
public class ShopUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }
    public static ShopUI Instance { get; private set; }

    private GameObject overlay;
    private Image itemIcon;
    private TMP_Text itemNameText;
    private TMP_Text itemDescriptionText;
    private TMP_Text priceText;
    private TMP_Text statusText;
    private Button buyButton;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("ShopUI needs the GoldHUD canvas created by GoldWallet.");
            enabled = false;
            return;
        }

        BuildUI(canvas.transform);
    }

    private void Start()
    {
        UIManager uiManager = FindObjectOfType<UIManager>(true);
        if (uiManager != null && uiManager.KoreanUiFont != null)
        {
            foreach (TMP_Text text in overlay.GetComponentsInChildren<TMP_Text>(true))
                text.font = uiManager.KoreanUiFont;
        }

        RefreshProduct();
    }

    private void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            SetOpen(false);
    }

    private void OnDestroy()
    {
        if (IsOpen)
        {
            IsOpen = false;
            Time.timeScale = previousTimeScale;
        }

        if (Instance == this)
            Instance = null;
    }

    private void BuildUI(Transform canvas)
    {
        overlay = new GameObject("ShopOverlay", typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(canvas, false);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image dimmer = overlay.GetComponent<Image>();
        dimmer.color = new Color(0f, 0f, 0f, 0.62f);
        dimmer.raycastTarget = true;

        GameObject window = new GameObject("ShopWindow", typeof(RectTransform), typeof(Image));
        window.transform.SetParent(overlay.transform, false);
        RectTransform windowRect = window.GetComponent<RectTransform>();
        SetCenteredRect(windowRect, Vector2.zero, new Vector2(580f, 390f));
        window.GetComponent<Image>().color = new Color(0.1f, 0.09f, 0.08f, 0.98f);

        CreateText(window.transform, "Title", "상점", new Vector2(0f, 155f), new Vector2(400f, 46f), 34f, TextAlignmentOptions.Center);

        Button closeButton = CreateButton(window.transform, "CloseButton", "×", new Vector2(250f, 155f), new Vector2(42f, 42f));
        closeButton.onClick.AddListener(() => SetOpen(false));

        GameObject iconObject = new GameObject("ItemIcon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(window.transform, false);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetCenteredRect(iconRect, new Vector2(-190f, 48f), new Vector2(84f, 84f));
        itemIcon = iconObject.GetComponent<Image>();
        itemIcon.preserveAspect = true;
        itemIcon.raycastTarget = false;

        itemNameText = CreateText(window.transform, "ItemName", "포션", new Vector2(-65f, 88f), new Vector2(300f, 40f), 26f, TextAlignmentOptions.MidlineLeft);
        itemDescriptionText = CreateText(window.transform, "ItemDescription", "체력을 회복합니다.", new Vector2(-65f, 46f), new Vector2(320f, 48f), 20f, TextAlignmentOptions.TopLeft);
        priceText = CreateText(window.transform, "Price", "가격 확인 중", new Vector2(-65f, -5f), new Vector2(300f, 38f), 22f, TextAlignmentOptions.MidlineLeft);

        buyButton = CreateButton(window.transform, "BuyButton", "구매", new Vector2(0f, -88f), new Vector2(220f, 54f));
        buyButton.onClick.AddListener(TryPurchasePotion);

        statusText = CreateText(window.transform, "Status", "", new Vector2(0f, -145f), new Vector2(500f, 34f), 18f, TextAlignmentOptions.Center);
        overlay.SetActive(false);
    }

    public void OpenShop()
    {
        SetOpen(true);
    }

    public void CloseShop()
    {
        SetOpen(false);
    }

    private void SetOpen(bool open)
    {
        if (overlay == null || IsOpen == open)
            return;

        if (open)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            RefreshProduct();
            statusText.text = string.Empty;
            overlay.SetActive(true);
            overlay.transform.SetAsLastSibling();
        }
        else
        {
            overlay.SetActive(false);
            Time.timeScale = previousTimeScale;
        }

        IsOpen = open;
    }

    private void RefreshProduct()
    {
        InventoryUI inventory = FindObjectOfType<InventoryUI>(true);
        ItemData potion = inventory != null ? inventory.ShopConsumable : null;

        if (potion == null)
        {
            itemNameText.text = "판매할 포션이 없습니다.";
            itemDescriptionText.text = "인벤토리의 회복 아이템 설정을 확인해 주세요.";
            priceText.text = "상품 없음";
            itemIcon.enabled = false;
            buyButton.interactable = false;
            return;
        }

        itemNameText.text = potion.itemName;
        itemDescriptionText.text = potion.description;
        itemIcon.sprite = potion.icon;
        itemIcon.enabled = potion.icon != null;
        int price = Mathf.Max(0, potion.shopPrice);
        priceText.text = price > 0 ? $"가격: {price} G" : "판매 준비 중";
        buyButton.interactable = price > 0;
    }

    private void TryPurchasePotion()
    {
        InventoryUI inventory = FindObjectOfType<InventoryUI>(true);
        ItemData potion = inventory != null ? inventory.ShopConsumable : null;

        if (potion == null || potion.itemType != ItemType.Consumable)
        {
            ShowStatus("구매할 소모품을 찾을 수 없습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        int price = Mathf.Max(0, potion.shopPrice);
        if (price <= 0)
        {
            ShowStatus("이 상품은 아직 판매하지 않습니다.", new Color(1f, 0.75f, 0.4f));
            return;
        }

        if (!inventory.CanAddItem(potion, 1))
        {
            ShowStatus("인벤토리에 공간이 없습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        GoldWallet wallet = GoldWallet.Instance;
        if (wallet == null || !wallet.TrySpend(price))
        {
            ShowStatus("골드가 부족합니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        if (!inventory.AddItem(potion, 1))
        {
            wallet.AddGold(price);
            ShowStatus("구매에 실패해 골드를 돌려드렸습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        ShowStatus($"{potion.itemName} 구매 완료!", new Color(0.72f, 1f, 0.72f));
    }

    private void ShowStatus(string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }

    private static TMP_Text CreateText(Transform parent, string name, string value, Vector2 position, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        SetCenteredRect((RectTransform)textObject.transform, position, size);

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(12f, fontSize - 4f);
        text.fontSizeMax = fontSize;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        SetCenteredRect((RectTransform)buttonObject.transform, position, size);

        Image background = buttonObject.GetComponent<Image>();
        background.color = new Color(0.32f, 0.23f, 0.12f, 0.96f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = background;
        CreateText(buttonObject.transform, "Label", label, Vector2.zero, size - new Vector2(12f, 8f), 20f, TextAlignmentOptions.Center);
        return button;
    }

    private static void SetCenteredRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
