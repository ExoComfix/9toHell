using UnityEngine;

public class EnemyAI : MonoBehaviour 
{
    [Header("Enemy Stats")]
    public float moveSpeed = 2.5f;
    public float burnoutDamage = 10f;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null )
        {
            playerTransform = player.transform;
        }
        if ( rb != null ) 
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

        }
    }
    void FixedUpdate()
    {
        if (playerTransform != null && rb != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.IncreaseBurnout(burnoutDamage);
                Destroy(gameObject);
            }
        }
    }
}
