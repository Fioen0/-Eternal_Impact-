using UnityEngine;

/// <summary>Editor-visible marker used by a room's enemy spawn settings.</summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = new Color(1f, 0.35f, 0.2f, 1f);
    [SerializeField, Min(0.05f)] private float gizmoRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.DrawLine(transform.position + Vector3.left * gizmoRadius, transform.position + Vector3.right * gizmoRadius);
        Gizmos.DrawLine(transform.position + Vector3.up * gizmoRadius, transform.position + Vector3.down * gizmoRadius);
    }
}
