using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("방 정보")]
    [SerializeField] private int roomIndex; // 방 번호 (0~3)
    [SerializeField] private Transform spawnPoint; // 플레이어 등장 위치
    [SerializeField] private GameObject exitDoor;  // 다음 방으로 가는 문/포탈

    public bool isCleared { get; private set; } = false;

    // 방 진입 시 실행
    public void EnterRoom(GameObject player)
    {
        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.position;
        }

        // 초기 진입 시 문 닫기
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    // 방 클리어 시 호출 (몬스터 모두 처치 시)
    public void ClearRoom()
    {
        isCleared = true;
        Debug.Log($"{roomIndex + 1}번 방 클리어!");

        // 문 열기
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }
    }
}