using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float speed = 7f;
    private float damage = 10f;
    private Vector2 moveDirection;
    private bool isReady = false;
    private Rigidbody2D rb;

    public void Setup(Vector3 direction, float damageAmount)
    {
        this.moveDirection = direction.normalized;
        this.damage = damageAmount;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.linearVelocity = moveDirection * speed;
        }
        isReady = true;
        CancelInvoke("Deactivate");
        Invoke("Deactivate", 4f);
    }
    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.IncreaseBurnout(damage);

                if (Camera.main != null)
                {
                    CameraShake shaker = Camera.main.GetComponent<CameraShake>();
                    if (shaker != null) shaker.Shake(0.15f, 0.15f);
                }
            }
            Deactivate();
        }
    }
}