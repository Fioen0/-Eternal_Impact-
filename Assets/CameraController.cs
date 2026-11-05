using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f; // 카메라 전환 속도

    void Start()
    {
        // 시작할 때 현재 카메라 위치를 초기 목표로 설정
        targetPosition = transform.position;
    }

    void Update()
    {
        // 목표 위치로 부드럽게 이동 (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // 방 이동 시 호출되는 함수
    public void MoveToNewRoom(Vector3 newCamPos)
    {
        targetPosition = newCamPos;
    }
}