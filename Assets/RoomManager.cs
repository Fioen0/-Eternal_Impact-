using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public enum RoomType { NormalCombat, Shop, Treasure, Boss }

    [Header("방 설정")]
    [SerializeField] private RoomType roomType = RoomType.NormalCombat;
    [SerializeField] private RoomMoveTrigger exitDoor; // 해당 방의 출구 문 트리거

    [Header("전투방 전용 (몬스터 부모)")]
    [SerializeField] private Transform monsterContainer; // 방 안에 배치된 몬스터들을 모아둔 부모 오브젝트

    private bool isCleared = false;

    void Start()
    {
        // 상점, 보물방 등 특수 방은 조건 없이 즉시 문 개방
        if (roomType == RoomType.Shop || roomType == RoomType.Treasure)
        {
            ClearRoom();
        }
        else
        {
            if (exitDoor != null) exitDoor.SetDoorOpen(false);
        }
    }

    void Update()
    {
        if (isCleared) return;

        // 일반 전투방 또는 보스방인 경우 몬스터 전멸 체크
        if (roomType == RoomType.NormalCombat || roomType == RoomType.Boss)
        {
            CheckMonsters();
        }
    }

    private void CheckMonsters()
    {
        if (monsterContainer != null)
        {
            // 몬스터 컨테이너 안의 남은 자식(몬스터) 수가 0개이면 클리어
            if (monsterContainer.childCount == 0)
            {
                ClearRoom();
            }
        }
    }

    private void ClearRoom()
    {
        isCleared = true;
        Debug.Log($"방 클리어 완료! 출구 문이 열렸습니다.");

        if (exitDoor != null)
        {
            exitDoor.SetDoorOpen(true);
        }
    }
}