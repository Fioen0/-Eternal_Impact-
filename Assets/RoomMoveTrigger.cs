using UnityEngine;

public class RoomMoveTrigger : MonoBehaviour
{
    [Header("이동할 방 설정")]
    [SerializeField] private RoomManager targetRoomManager;
    [SerializeField] private Transform targetRoomPosition; // RoomManager가 없을 때 사용하는 기존 방식
    [SerializeField] private Vector3 targetCameraPosition;

    private bool isOpen = false;
    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        if (doorCollider != null) doorCollider.enabled = false;
    }

    public void SetDoorOpen(bool open)
    {
        isOpen = open;
        if (doorCollider != null) doorCollider.enabled = open;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen || !other.CompareTag("Player")) return;

        if (targetRoomManager != null)
        {
            targetRoomManager.EnterRoom(other.gameObject);
            targetCameraPosition = targetRoomManager.transform.position;
        }
        else if (targetRoomPosition != null)
        {
            other.transform.position = targetRoomPosition.position;
        }

        Camera mainCamera = Camera.main;
        CameraController cam = mainCamera != null ? mainCamera.GetComponent<CameraController>() : null;
        if (cam != null)
            cam.MoveToNewRoom(new Vector3(targetCameraPosition.x, targetCameraPosition.y, -10f));

        Debug.Log("다음 방으로 이동 완료!");
    }
}
