using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private bool isExitToNextFloor = false; // 층의 마지막 방이라 다음 층 씬으로 가야 하는지 여부
    [SerializeField] private string nextFloorSceneName = "Floor_2"; // 다음 층 씬 이름
    [SerializeField] private Transform nextRoomSpawnPoint; // 같은 씬 안에서 다음 방 스폰 위치

    private bool isOpen = false;
    private Collider2D doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
    }

    // RoomManager에서 조건 달성 시 호출
    public void SetDoorActive(bool active)
    {
        isOpen = active;
        if (doorCollider != null)
        {
            doorCollider.enabled = active; // 문 영역 트리거 켜기/끄기
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 문이 열려있고 플레이어가 들어온 경우
        if (isOpen && other.CompareTag("Player"))
        {
            if (isExitToNextFloor)
            {
                // 다음 층 씬으로 전환
                if (DungeonProgress.Instance != null)
                {
                    DungeonProgress.Instance.LoadNextFloor(nextFloorSceneName);
                }
            }
            else if (nextRoomSpawnPoint != null)
            {
                // 같은 층 안에서 다음 방 위치로 플레이어 이동
                other.transform.position = nextRoomSpawnPoint.position;
            }
        }
    }
}