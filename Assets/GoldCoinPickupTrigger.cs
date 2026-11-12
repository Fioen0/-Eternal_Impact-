using UnityEngine;

/// <summary>Forwards the trigger overlap to the coin's physics root.</summary>
public class GoldCoinPickupTrigger : MonoBehaviour
{
    private GoldCoinPickup coin;

    private void Awake() => coin = GetComponentInParent<GoldCoinPickup>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (coin != null)
            coin.TryCollect(other);
    }
}
