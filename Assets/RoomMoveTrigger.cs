using UnityEngine;

public class RoomMoveTrigger : MonoBehaviour
{
    [Header("이동할 다음 방 좌표 설정")]
    [SerializeField] private Transform targetRoomPosition; // 다음 방의 중심 또는 스폰 좌표
    [SerializeField] private Vector3 targetCameraPosition;  // 다음 방에서의 카메라 위치 (Z축은 보통 -10)

    private bool isOpen = false;
    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        // 처음에는 문이 닫혀있으므로 콜라이더 비활성화
        if (doorCollider != null) doorCollider.enabled = false;
    }

    // RoomManager에서 미션 클리어 시 호출
    public void SetDoorOpen(bool open)
    {
        isOpen = open;
        if (doorCollider != null)
        {
            doorCollider.enabled = open;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            // 1. 플레이어를 다음 방 위치로 이동
            if (targetRoomPosition != null)
            {
                other.transform.position = targetRoomPosition.position;
            }

            // 2. 메인 카메라의 목표 위치를 다음 방 카메라 위치로 변경
            CameraController cam = Camera.main.GetComponent<CameraController>();
            if (cam != null)
            {
                cam.MoveToNewRoom(new Vector3(targetCameraPosition.x, targetCameraPosition.y, -10f));
            }

            Debug.Log("다음 방으로 이동 완료!");
        }
    }
}