using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartListUI : MonoBehaviour
{
    [Header("스프라이트 설정")]
    [SerializeField] private Sprite fullHeart;   // 채워진 하트 이미지
    [SerializeField] private Sprite halfHeart;   // 반쪽 하트 이미지 (없다면 full/empty만 사용 가능)
    [SerializeField] private Sprite emptyHeart;  // 빈 하트 이미지

    [Header("UI 연결")]
    [SerializeField] private GameObject heartPrefab; // 아까 만든 HeartUI 프리팹
    [SerializeField] private Transform heartContainer; // 하트들이 들어갈 HeartContainer
    [SerializeField] private CharacterStats playerStats; // 플레이어 스탯 참조

    private List<Image> heartImages = new List<Image>();

    void Start()
    {
        // 플레이어 자동 감지
        if (playerStats == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<CharacterStats>();
            }
        }

        // 초기 하트 생성 및 UI 업데이트
        InitHearts();
        UpdateHearts();
    }

    void Update()
    {
        // 실시간 체력 변경 감지
        UpdateHearts();
    }

    // 최대 체력만큼 하트 UI 생성
    private void InitHearts()
    {
        if (playerStats == null || heartPrefab == null || heartContainer == null) return;

        // 기존에 생성된 하트가 있다면 삭제
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // 최대 체력(예: 5)만큼 하트 프리팹 생성
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

    // 현재 체력에 따라 하트 이미지 교체
    private void UpdateHearts()
    {
        if (playerStats == null) return;

        // 플레이어의 MaxHealth가 바뀌었을 경우 재생성
        if (heartImages.Count != playerStats.MaxHealth)
        {
            InitHearts();
        }

        int currentHP = playerStats.CurrentHealth;

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHP)
            {
                // 현재 체력 범위 안이면 꽉 찬 하트
                heartImages[i].sprite = fullHeart;
                heartImages[i].color = Color.white;
            }
            else
            {
                // 체력이 깎인 부분은 빈 하트 (또는 emptyHeart 스프라이트가 없다면 어둡게 처리)
                if (emptyHeart != null)
                {
                    heartImages[i].sprite = emptyHeart;
                    heartImages[i].color = Color.white;
                }
                else
                {
                    heartImages[i].sprite = fullHeart;
                    heartImages[i].color = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 어두운 투명색
                }
            }
        }
    }
}