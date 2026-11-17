using System.Collections.Generic;
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
    private Transform productList;
    private readonly List<GameObject> productButtons = new List<GameObject>();
    private ItemData[] currentStock = new ItemData[0];
    private ItemData selectedItem;
    private int healthRestorePrice;
    private bool healthRestoreSelected;
    private bool playerNearShop;
    private TMP_Text interactionPrompt;
    private TMP_FontAsset koreanUiFont;
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
            koreanUiFont = uiManager.KoreanUiFont;
            foreach (TMP_Text text in overlay.GetComponentsInChildren<TMP_Text>(true))
                text.font = koreanUiFont;
            interactionPrompt.font = koreanUiFont;
        }
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
        SetCenteredRect(windowRect, Vector2.zero, new Vector2(820f, 520f));
        window.GetComponent<Image>().color = new Color(0.1f, 0.09f, 0.08f, 0.98f);

        CreateText(window.transform, "Title", "상점", new Vector2(0f, 215f), new Vector2(400f, 46f), 34f, TextAlignmentOptions.Center);

        Button closeButton = CreateButton(window.transform, "CloseButton", "×", new Vector2(365f, 215f), new Vector2(42f, 42f));
        closeButton.onClick.AddListener(() => SetOpen(false));

        GameObject listPanel = new GameObject("ProductList", typeof(RectTransform), typeof(Image));
        listPanel.transform.SetParent(window.transform, false);
        SetCenteredRect(listPanel.GetComponent<RectTransform>(), new Vector2(-255f, -8f), new Vector2(270f, 382f));
        listPanel.GetComponent<Image>().color = new Color(0.04f, 0.035f, 0.03f, 0.8f);
        productList = listPanel.transform;

        GameObject iconObject = new GameObject("ItemIcon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(window.transform, false);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        SetCenteredRect(iconRect, new Vector2(-70f, 112f), new Vector2(84f, 84f));
        itemIcon = iconObject.GetComponent<Image>();
        itemIcon.preserveAspect = true;
        itemIcon.raycastTarget = false;

        itemNameText = CreateText(window.transform, "ItemName", "상품을 선택해 주세요.", new Vector2(145f, 135f), new Vector2(370f, 48f), 26f, TextAlignmentOptions.MidlineLeft);
        itemDescriptionText = CreateText(window.transform, "ItemDescription", "", new Vector2(145f, 48f), new Vector2(370f, 112f), 20f, TextAlignmentOptions.TopLeft);
        priceText = CreateText(window.transform, "Price", "", new Vector2(145f, -48f), new Vector2(370f, 38f), 22f, TextAlignmentOptions.MidlineLeft);

        buyButton = CreateButton(window.transform, "BuyButton", "구매", new Vector2(145f, -125f), new Vector2(230f, 54f));
        buyButton.onClick.AddListener(TryPurchaseSelected);

        statusText = CreateText(window.transform, "Status", "", new Vector2(80f, -205f), new Vector2(690f, 34f), 18f, TextAlignmentOptions.Center);
        overlay.SetActive(false);

        GameObject promptObject = new GameObject("ShopInteractionPrompt", typeof(RectTransform), typeof(TextMeshProUGUI));
        promptObject.transform.SetParent(canvas, false);
        RectTransform promptRect = promptObject.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0f);
        promptRect.anchorMax = new Vector2(0.5f, 0f);
        promptRect.pivot = new Vector2(0.5f, 0f);
        promptRect.anchoredPosition = new Vector2(0f, 90f);
        promptRect.sizeDelta = new Vector2(400f, 54f);
        interactionPrompt = promptObject.GetComponent<TMP_Text>();
        interactionPrompt.text = "[E] 상점 이용";
        interactionPrompt.fontSize = 26f;
        interactionPrompt.fontStyle = FontStyles.Bold;
        interactionPrompt.color = Color.white;
        interactionPrompt.alignment = TextAlignmentOptions.Center;
        interactionPrompt.raycastTarget = false;
        promptObject.SetActive(false);
    }

    public void OpenShop(ItemData[] stock, int fullHealPrice)
    {
        currentStock = stock ?? new ItemData[0];
        healthRestorePrice = Mathf.Max(0, fullHealPrice);
        PopulateProductList();
        SetOpen(true);

        ItemData firstProduct = null;
        foreach (ItemData item in currentStock)
        {
            if (item != null && (item.itemType == ItemType.Consumable || item.itemType == ItemType.Relic))
            {
                firstProduct = item;
                break;
            }
        }

        if (firstProduct != null)
            SelectItem(firstProduct);
        else if (healthRestorePrice > 0)
            SelectHealthRestore();
        else
            ShowStatus("판매 중인 상품이 없습니다.", new Color(1f, 0.75f, 0.4f));
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
            statusText.text = string.Empty;
            overlay.SetActive(true);
            overlay.transform.SetAsLastSibling();
            interactionPrompt.gameObject.SetActive(false);
        }
        else
        {
            overlay.SetActive(false);
            Time.timeScale = previousTimeScale;
            interactionPrompt.gameObject.SetActive(playerNearShop);
        }

        IsOpen = open;
    }

    public void SetInteractionAvailable(bool available)
    {
        playerNearShop = available;
        if (interactionPrompt != null && !IsOpen)
            interactionPrompt.gameObject.SetActive(available);
    }

    private void PopulateProductList()
    {
        foreach (GameObject oldButton in productButtons)
        {
            if (oldButton != null) Destroy(oldButton);
        }
        productButtons.Clear();

        int row = 0;
        foreach (ItemData item in currentStock)
        {
            if (item == null || (item.itemType != ItemType.Consumable && item.itemType != ItemType.Relic)) continue;
            int price = Mathf.Max(0, item.shopPrice);
            string label = price > 0 ? $"{item.itemName}  ·  {price} G" : $"{item.itemName}  ·  가격 미설정";
            Button button = CreateButton(productList, $"Product_{row}", label,
                new Vector2(0f, 158f - row * 58f), new Vector2(240f, 48f));
            button.interactable = price > 0;
            ApplyKoreanFont(button.transform);
            ItemData selected = item;
            button.onClick.AddListener(() => SelectItem(selected));
            productButtons.Add(button.gameObject);
            row++;
        }

        if (healthRestorePrice > 0)
        {
            Button healOption = CreateButton(productList, "FullHealProduct", $"체력 완전 회복  ·  {healthRestorePrice} G",
                new Vector2(0f, 158f - row * 58f), new Vector2(240f, 48f));
            ApplyKoreanFont(healOption.transform);
            healOption.onClick.AddListener(SelectHealthRestore);
            productButtons.Add(healOption.gameObject);
        }
    }

    private void SelectItem(ItemData item)
    {
        selectedItem = item;
        healthRestoreSelected = false;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = string.IsNullOrWhiteSpace(item.description) ? "설명이 없습니다." : item.description;
        itemIcon.sprite = item.icon;
        itemIcon.enabled = item.icon != null;
        int price = Mathf.Max(0, item.shopPrice);
        priceText.text = price > 0 ? $"가격: {price} G" : "가격이 설정되지 않았습니다.";
        buyButton.interactable = price > 0;
        statusText.text = string.Empty;
    }

    private void SelectHealthRestore()
    {
        selectedItem = null;
        healthRestoreSelected = true;
        itemNameText.text = "체력 완전 회복";
        itemDescriptionText.text = "현재 체력을 최대 체력까지 회복합니다.";
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        priceText.text = $"가격: {healthRestorePrice} G";
        CharacterStats stats = FindPlayerStats();
        buyButton.interactable = healthRestorePrice > 0 && stats != null && stats.CurrentHealth < stats.MaxHealth && !stats.isDead;
        statusText.text = string.Empty;
    }

    private void TryPurchaseSelected()
    {
        if (healthRestoreSelected)
        {
            TryPurchaseHealthRestore();
            return;
        }

        InventoryUI inventory = FindObjectOfType<InventoryUI>(true);
        ItemData item = selectedItem;
        if (inventory == null || item == null || (item.itemType != ItemType.Consumable && item.itemType != ItemType.Relic))
        {
            ShowStatus("구매할 상품을 찾을 수 없습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        int price = Mathf.Max(0, item.shopPrice);
        if (price <= 0)
        {
            ShowStatus("이 상품은 아직 판매하지 않습니다.", new Color(1f, 0.75f, 0.4f));
            return;
        }

        if (!inventory.CanAddItem(item, 1))
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

        if (!inventory.AddItem(item, 1))
        {
            wallet.AddGold(price);
            ShowStatus("구매에 실패해 골드를 돌려드렸습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        ShowStatus($"{item.itemName} 구매 완료!", new Color(0.72f, 1f, 0.72f));
    }

    private void TryPurchaseHealthRestore()
    {
        CharacterStats stats = FindPlayerStats();
        if (stats == null || stats.isDead)
        {
            ShowStatus("플레이어 체력을 확인할 수 없습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        if (stats.CurrentHealth >= stats.MaxHealth)
        {
            ShowStatus("체력이 이미 가득 찼습니다.", new Color(1f, 0.75f, 0.4f));
            buyButton.interactable = false;
            return;
        }

        GoldWallet wallet = GoldWallet.Instance;
        if (wallet == null || !wallet.TrySpend(healthRestorePrice))
        {
            ShowStatus("골드가 부족합니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        if (!stats.Heal(stats.MaxHealth))
        {
            wallet.AddGold(healthRestorePrice);
            ShowStatus("회복에 실패해 골드를 돌려드렸습니다.", new Color(1f, 0.55f, 0.45f));
            return;
        }

        ShowStatus("체력을 모두 회복했습니다!", new Color(0.72f, 1f, 0.72f));
        buyButton.interactable = false;
    }

    private static CharacterStats FindPlayerStats()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player != null ? player.GetComponentInParent<CharacterStats>() : null;
    }

    private void ApplyKoreanFont(Transform root)
    {
        if (koreanUiFont == null) return;
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            text.font = koreanUiFont;
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
