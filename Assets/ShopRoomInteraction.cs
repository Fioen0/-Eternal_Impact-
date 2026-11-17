using System.Collections.Generic;
using UnityEngine;

/// <summary>Opens the shop while the player is inside its interaction trigger.</summary>
[RequireComponent(typeof(BoxCollider2D))]
public class ShopRoomInteraction : MonoBehaviour
{
    [Header("상점 상품")]
    [SerializeField] private ItemData[] stock;
    [SerializeField, Min(0)] private int fullHealPrice = 25;

    [Header("상호작용")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private readonly HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();
    private Collider2D interactionCollider;

    private void Awake()
    {
        interactionCollider = GetComponent<Collider2D>();
        interactionCollider.isTrigger = true;
    }

    private void Update()
    {
        if (playerColliders.Count == 0 || ShopUI.IsOpen || !Input.GetKeyDown(interactKey))
            return;

        if (ShopUI.Instance == null)
        {
            Debug.LogError("상점 UI가 준비되지 않았습니다. GoldWallet이 생성됐는지 확인해 주세요.", this);
            return;
        }

        ShopUI.Instance.OpenShop(stock, fullHealPrice);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayerCollider(other)) return;
        playerColliders.Add(other);
        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!playerColliders.Remove(other)) return;
        UpdatePrompt();
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        if (ShopUI.Instance != null)
            ShopUI.Instance.SetInteractionAvailable(false);
    }

    private static bool IsPlayerCollider(Collider2D collider)
    {
        CharacterStats stats = collider.GetComponentInParent<CharacterStats>();
        return stats != null && stats.CompareTag("Player");
    }

    private void UpdatePrompt()
    {
        if (ShopUI.Instance != null)
            ShopUI.Instance.SetInteractionAvailable(playerColliders.Count > 0);
    }
}
