using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("이펙트가 유지될 시간(초)입니다. 이 시간이 지나면 오브젝트가 파괴됩니다.")]
    [SerializeField] private float destroyTime = 0.5f;

    void Start()
    {
        // 지정된 시간(destroyTime)이 지나면 이 게임 오브젝트를 자동으로 삭제합니다.
        Destroy(gameObject, destroyTime);
    }
}