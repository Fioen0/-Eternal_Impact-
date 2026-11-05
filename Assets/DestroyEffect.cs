using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField] private float destroyTime = 0.15f; // 이펙트가 유지될 시간 (초)

    void Start()
    {
        // 지정된 시간이 지나면 이 오브젝트를 파괴합니다.
        Destroy(gameObject, destroyTime);
    }
}