using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("통합 UI 창 전체 패널")]
    [SerializeField] private GameObject mainUIPanel; // MainUIPanel 연결

    [Header("탭별 내용 패널")]
    [SerializeField] private GameObject inventoryPanel; // Panel_Inventory 연결
    [FormerlySerializedAs("equipmentPanel")]
    [SerializeField] private GameObject relicPanel; // Panel_Equipment (유물 장착 화면) 연결
    [SerializeField] private GameObject skillPanel;     // Panel_Skill 연결
    [SerializeField] private TMP_FontAsset koreanUiFont;

    void Start()
    {
        SetupTabButtons();
        // 게임 시작 시 통합 창은 꺼진 상태로 시작
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
        }
    }

    private void SetupTabButtons()
    {
        if (mainUIPanel == null) return;

        Button[] buttons = mainUIPanel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            switch (button.gameObject.name)
            {
                case "Btn_Inventory":
                    SetButtonLabel(button, "아이템", koreanUiFont);
                    button.onClick.AddListener(ShowInventoryTab);
                    break;
                case "Btn_Equipment":
                case "Btn_Relic":
                    SetButtonLabel(button, "유물", koreanUiFont);
                    button.onClick.AddListener(ShowRelicTab);
                    break;
                case "Btn_Skill":
                    SetButtonLabel(button, "스킬", koreanUiFont);
                    button.onClick.AddListener(ShowSkillTab);
                    break;
            }
        }
    }

    private static void SetButtonLabel(Button button, string label, TMP_FontAsset font)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            if (font != null) text.font = font;
            text.text = label;
        }
    }

    void Update()
    {
        // Tab 키를 누르면 통합 UI 창 열기/닫기 토글
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (mainUIPanel != null)
            {
                bool isOpen = !mainUIPanel.activeSelf;
                mainUIPanel.SetActive(isOpen);

                // 창이 열릴 때 기본적으로 인벤토리 탭을 먼저 보여줌
                if (isOpen)
                {
                    ShowInventoryTab();
                }
            }
        }
    }

    // 1. 인벤토리 탭 활성화
    public void ShowInventoryTab()
    {
        if (inventoryPanel) inventoryPanel.SetActive(true);
        if (relicPanel) relicPanel.SetActive(false);
        if (skillPanel) skillPanel.SetActive(false);
    }

    // 2. 유물 장착 화면
    public void ShowRelicTab()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
        if (relicPanel) relicPanel.SetActive(true);
        if (skillPanel) skillPanel.SetActive(false);
    }

    // Keep existing Button OnClick references working in the saved scene.
    public void ShowEquipmentTab() => ShowRelicTab();

    // 3. 스킬 탭 활성화 (추후 확장용)
    public void ShowSkillTab()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
        if (relicPanel) relicPanel.SetActive(false);
        if (skillPanel) skillPanel.SetActive(true);
    }
}
