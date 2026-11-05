using UnityEngine;

/// <summary>A dropped coin pile that adds gold when the player touches it.</summary>
public class GoldCoinPickup : MonoBehaviour
{
    private int amount;
    private bool collected;

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
        pickup.transform.position = position + Vector3.up * 0.35f;
        pickup.transform.localScale = Vector3.one * 1.25f;

        SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
        renderer.sprite = coinSprite;
        renderer.sortingOrder = 5;

        CircleCollider2D trigger = pickup.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.45f;

        Rigidbody2D body = pickup.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        GoldCoinPickup coin = pickup.AddComponent<GoldCoinPickup>();
        coin.amount = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
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
