using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    [Header("전체 층 설정")]
    [SerializeField] private int maxFloors = 5;
    [SerializeField] private int currentFloor = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 다음 층으로 이동
    public void GoToNextFloor()
    {
        if (currentFloor < maxFloors)
        {
            currentFloor++;
            Debug.Log($"[{currentFloor}층]에 진입했습니다.");
            
            // 씬을 층마다 나눈 경우 씬 로드 (예: "Floor_2")
            // SceneManager.LoadScene($"Floor_{currentFloor}");
        }
        else
        {
            Debug.Log("🎉 던전의 모든 5개 층을 완전 클리어하셨습니다!");
        }
    }
}