using UnityEngine;

public class RoomExit : MonoBehaviour
{
    [Header("이동할 다음 씬 이름")]
    [SerializeField] private string nextSceneName = "Floor1_Room2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 문 영역에 들어왔을 때
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextRoom(nextSceneName);
            }
        }
    }
}