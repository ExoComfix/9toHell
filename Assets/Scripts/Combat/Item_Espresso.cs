using UnityEngine;
using System.Collections;

public class Item_Espresso : MonoBehaviour
{
    [Header("Espresso Buff Settings")]
    public float speedMultiplier = 1.5f;
    public float duration = 5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D pickupCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pickupCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (pickupCollider != null) pickupCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.StartCoroutine(ApplySpeedBuff(player));
            }

            Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowSynergyNotification("<color=#FF9933><b>☕ ÇİFT SHOT ESPRESSO!</b></color>\nHareket hızı 5 saniyeliğine uçuşa geçti!");
            }

            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (pickupCollider != null) pickupCollider.enabled = false;
            StartCoroutine(ReturnToPoolAfterDelay(duration + 1f));
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator ApplySpeedBuff(PlayerController player)
    {
        player.ModifyMoveSpeed(speedMultiplier);

        yield return new WaitForSeconds(duration);
        if (player != null)
        {
            player.ModifyMoveSpeed(1f / speedMultiplier);
        }
    }
}