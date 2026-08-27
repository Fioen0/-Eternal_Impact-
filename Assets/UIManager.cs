using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("통합 UI 창 전체 패널")]
    [SerializeField] private GameObject mainUIPanel; // MainUIPanel 연결

    [Header("탭별 내용 패널")]
    [SerializeField] private GameObject inventoryPanel; // Panel_Inventory 연결
    [SerializeField] private GameObject equipmentPanel; // Panel_Equipment 연결
    [SerializeField] private GameObject skillPanel;     // Panel_Skill 연결

    void Start()
    {
        // 게임 시작 시 통합 창은 꺼진 상태로 시작
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
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
        if (equipmentPanel) equipmentPanel.SetActive(false);
        if (skillPanel) skillPanel.SetActive(false);
    }

    // 2. 장비 탭 활성화 (무기, 성유물 3개 슬롯 관리 화면)
    public void ShowEquipmentTab()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
        if (equipmentPanel) equipmentPanel.SetActive(true);
        if (skillPanel) skillPanel.SetActive(false);
    }

    // 3. 스킬 탭 활성화 (추후 확장용)
    public void ShowSkillTab()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
        if (equipmentPanel) equipmentPanel.SetActive(false);
        if (skillPanel) skillPanel.SetActive(true);
    }
}