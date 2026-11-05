using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartListUI : MonoBehaviour
{
    [Header("스프라이트 설정")]
    [SerializeField] private Sprite fullHeart;   // 채워진 하트 이미지
    [SerializeField] private Sprite halfHeart;   // 반쪽 하트 이미지 (선택 사항)
    [SerializeField] private Sprite emptyHeart;  // 빈 하트 이미지

    [Header("UI 및 스탯 연결")]
    [SerializeField] private GameObject heartPrefab;     // 생성할 하트 이미지 프리팹 (★ 필수!)
    [SerializeField] private Transform heartContainer;   // 하트들이 배치될 컨테이너
    [SerializeField] private CharacterStats playerStats; // 플레이어 스탯 스크립트

    private List<Image> heartImages = new List<Image>();

    void Start()
    {
        // 1. 컨테이너 자동 설정
        if (heartContainer == null)
        {
            heartContainer = this.transform;
        }

        // 2. CharacterStats 자동 탐색 (오브젝트 직접 지정 안 했을 경우)
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerStats = player.GetComponentInParent<CharacterStats>();
            if (playerStats == null)
            {
                Debug.LogError("[HeartListUI] 씬에서 CharacterStats 컴포넌트를 찾을 수 없습니다! Player 오브젝트를 확인해 주세요.");
                return;
            }
        }

        // 3. 초기 하트 UI 생성 및 업데이트
        InitHearts();
        UpdateHearts();
        playerStats.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnDestroy()
    {
        if (playerStats != null) playerStats.OnHealthChanged.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged(int currentHealth, int maxHealth) => UpdateHearts();

    // 하트 UI 프리팹 동적 생성
    private void InitHearts()
    {
        if (heartPrefab == null)
        {
            Debug.LogError("[HeartListUI] 'Heart Prefab'이 비어 있습니다! Inspector에서 HeartUI 프리팹을 할당해 주세요.");
            return;
        }

        if (playerStats == null || heartContainer == null) return;

        // 기존에 있던 하트 UI 오브젝트 제거
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // MaxHealth 수치만큼 하트 프리팹 생성
        for (int i = 0; i < playerStats.MaxHealth; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            Image heartImage = newHeart.GetComponent<Image>();

            if (heartImage != null)
            {
                heartImages.Add(heartImage);
            }
        }
    }

    // 현재 체력 수치에 맞춰 하트 그래픽 갱신
    private void UpdateHearts()
    {
        if (playerStats == null) return;

        // 최대 체력이 변경되었거나 슬롯 개수가 안 맞으면 재생성
        if (heartImages.Count != playerStats.MaxHealth)
        {
            InitHearts();
        }

        int currentHP = playerStats.CurrentHealth;

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHP)
            {
                // 현재 체력 이하: 빨간 하트
                heartImages[i].sprite = fullHeart;
                heartImages[i].color = Color.white;
            }
            else
            {
                // 깎인 체력: 빈 하트 적용
                if (emptyHeart != null)
                {
                    heartImages[i].sprite = emptyHeart;
                    heartImages[i].color = Color.white;
                }
                else
                {
                    // 빈 하트 스프라이트가 없는 경우 어두운 반투명 처리
                    heartImages[i].sprite = fullHeart;
                    heartImages[i].color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                }
            }
        }
    }
}
