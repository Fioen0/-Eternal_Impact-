using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField, Min(0)] private int damage = 1;
    [SerializeField, Min(0.1f)] private float lifetime = 8f;
    [SerializeField] private LayerMask obstacleLayers;

    private Collider2D projectileCollider;
    private Rigidbody2D body;

    private void Awake()
    {
        projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, float speed)
    {
        body.velocity = direction.normalized * Mathf.Max(0f, speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterStats targetStats = other.GetComponentInParent<CharacterStats>();
        if (targetStats == null || !targetStats.CompareTag("Player"))
        {
            if ((obstacleLayers.value & (1 << other.gameObject.layer)) != 0)
                Destroy(gameObject);
            return;
        }

        PlayerSkillController playerSkills = targetStats.GetComponent<PlayerSkillController>();
        if (playerSkills != null && playerSkills.IsParrying)
        {
            Debug.Log("패링으로 적 투사체를 제거했습니다.", targetStats);
            Destroy(gameObject);
            return;
        }

        targetStats.TakeDamage(damage);
        Destroy(gameObject);
    }
}
