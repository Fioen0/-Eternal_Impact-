using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonProgress : MonoBehaviour
{
    public static DungeonProgress Instance { get; private set; }

    [Header("던전 진행 정보")]
    [SerializeField] private int currentFloor = 1;
    [SerializeField] private int maxFloor = 5;

    private void Awake()
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

    public void LoadNextFloor(string sceneName)
    {
        if (currentFloor < maxFloor)
        {
            currentFloor++;
            Debug.Log($"[{currentFloor}층] 로딩 시작");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("🎉 5층까지 모든 던전을 클리어했습니다!");
        }
    }
}