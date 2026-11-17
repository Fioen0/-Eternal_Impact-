using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private PlatformEffector2D platformEffector;

    public Collider2D PlatformCollider => platformCollider;

    private void Awake()
    {
        ConfigurePlatform();
    }

    private void Reset()
    {
        ConfigurePlatform();
    }

    private void ConfigurePlatform()
    {
        platformCollider = GetComponent<Collider2D>();
        platformEffector = GetComponent<PlatformEffector2D>();

        if (platformCollider != null)
            platformCollider.usedByEffector = true;

        if (platformEffector != null)
        {
            platformEffector.useOneWay = true;
            platformEffector.surfaceArc = 180f;
            platformEffector.useSideFriction = false;
            platformEffector.useSideBounce = false;
        }
    }
}
