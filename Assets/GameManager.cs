using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("던전 진행 상태")]
    [SerializeField] private int currentFloor = 1; // 현재 층 (1~5)
    [SerializeField] private int currentRoom = 1;  // 현재 방 (1~4)

    public int CurrentFloor => currentFloor;
    public int CurrentRoom => currentRoom;

    private void Awake()
    {
        // 씬이 바뀌어도 파괴되지 않는 싱글톤 생성
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

    // 다음 방/씬으로 이동하는 함수
    public void LoadNextRoom(string nextSceneName)
    {
        currentRoom++;
        
        // 4번째 방을 넘어가면 다음 층으로 이동
        if (currentRoom > 4)
        {
            currentRoom = 1;
            currentFloor++;
        }

        Debug.Log($"[{currentFloor}층 - {currentRoom}번 방] 로딩 중... (씬: {nextSceneName})");
        SceneManager.LoadScene(nextSceneName);
    }
}