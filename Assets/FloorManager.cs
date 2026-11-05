using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [Header("층 정보")]
    [SerializeField] private int floorLevel = 1; // 현재 층수 (1~5)
    [SerializeField] private List<Room> rooms = new List<Room>(); // 해당 층의 방 목록

    private int currentRoomIndex = 0;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartFloor();
    }

    // 층 시작
    public void StartFloor()
    {
        currentRoomIndex = 0;
        if (rooms.Count > 0)
        {
            rooms[currentRoomIndex].EnterRoom(player);
        }
    }

    // 다음 방으로 이동
    public void GoToNextRoom()
    {
        currentRoomIndex++;

        if (currentRoomIndex < rooms.Count)
        {
            rooms[currentRoomIndex].EnterRoom(player);
        }
        else
        {
            Debug.Log($"{floorLevel}층의 모든 방을 클리어했습니다! 다음 층으로 이동합니다.");
            // DungeonManager를 통해 다음 층 호출
            DungeonManager.Instance?.GoToNextFloor();
        }
    }

    // 현재 방 클리어 처리 테스트용
    public void ClearCurrentRoom()
    {
        if (currentRoomIndex < rooms.Count)
        {
            rooms[currentRoomIndex].ClearRoom();
        }
    }
}