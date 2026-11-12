using UnityEngine;

/// <summary>A dropped coin pile that adds gold when the player touches it.</summary>
public class GoldCoinPickup : MonoBehaviour
{
    private const string CoinLayerName = "Coin";
    private const string GroundLayerName = "Ground";
    private int amount;
    private bool collected;
    private static bool coinCollisionRulesConfigured;

    public static void Spawn(Vector3 position, int amount)
    {
        if (amount <= 0)
            return;

        Sprite coinSprite = Resources.Load<Sprite>("gold_coin");
        if (coinSprite == null)
        {
            Debug.LogError("Gold coin sprite was not found at Resources/gold_coin.");
            return;
        }

        GameObject pickup = new GameObject("GoldCoinPickup");
        int coinLayer = LayerMask.NameToLayer(CoinLayerName);
        int groundLayer = LayerMask.NameToLayer(GroundLayerName);
        if (coinLayer < 0 || groundLayer < 0)
        {
            Debug.LogError("Gold coin physics needs the 'Coin' and 'Ground' layers.");
            Destroy(pickup);
            return;
        }

        ConfigureCoinCollisions(coinLayer, groundLayer);
        pickup.layer = coinLayer;
        pickup.transform.position = position + Vector3.up * 0.35f;
        pickup.transform.localScale = Vector3.one * 1.25f;

        GoldCoinPickup coin = pickup.AddComponent<GoldCoinPickup>();
        coin.amount = amount;

        GameObject visual = new GameObject("CoinVisual");
        visual.transform.SetParent(pickup.transform, false);
        visual.transform.localScale = Vector3.one * 2.5f;

        SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
        renderer.sprite = coinSprite;
        renderer.sortingOrder = 5;

        CircleCollider2D groundCollider = pickup.AddComponent<CircleCollider2D>();
        groundCollider.radius = 0.35f;

        GameObject pickupTrigger = new GameObject("PickupTrigger");
        pickupTrigger.transform.SetParent(pickup.transform, false);
        CircleCollider2D trigger = pickupTrigger.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.65f;
        pickupTrigger.AddComponent<GoldCoinPickupTrigger>();

        Rigidbody2D body = pickup.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 1f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

    }

    private static void ConfigureCoinCollisions(int coinLayer, int groundLayer)
    {
        if (coinCollisionRulesConfigured) return;

        // Coin 오브젝트의 물리 콜라이더는 Ground에만 닿게 합니다.
        for (int layer = 0; layer < 32; layer++)
            Physics2D.IgnoreLayerCollision(coinLayer, layer, layer != groundLayer);

        coinCollisionRulesConfigured = true;
    }

    public void TryCollect(Collider2D other)
    {
        if (collected)
            return;

        CharacterStats playerStats = other.GetComponentInParent<CharacterStats>();
        if (playerStats == null || !playerStats.CompareTag("Player") || playerStats.isDead)
            return;

        if (GoldWallet.Instance == null)
            return;

        collected = true;
        GoldWallet.Instance.AddGold(amount);
        Destroy(gameObject);
    }
}
