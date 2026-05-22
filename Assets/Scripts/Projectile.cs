using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    [Header("Synergy Settings")]
    public float explosionRadius = 2.5f;
    public LayerMask enemyLayer;

    private GameObject fxPrefab;
    private GameObject explosionFXPrefab;

    public void Setup(Vector3 direction)
    {
        this.moveDirection = direction;
        fxPrefab = Resources.Load<GameObject>("FX_EmailDestroy");
        explosionFXPrefab = Resources.Load<GameObject>("FX_Explosion");
        CancelInvoke("DeactivateProjectile");
        Invoke("DeactivateProjectile", lifetime);
    }
    void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }
    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (fxPrefab)
            {
                GameObject effect = Instantiate(fxPrefab, collision.transform.position, Quaternion.identity);
                Destroy(effect, 0.5f);
            }
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            PlayerController player = playerObj != null ? playerObj.GetComponent<PlayerController>() : null;

            if (player != null)
            {
                if (player.isOvertimeSynergyActive)
                {
                    Collider2D[] surroundedEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
                    foreach (Collider2D enemy in surroundedEnemies)
                    {
                        EnemyAI currentEnemy = enemy.GetComponent<EnemyAI>();
                        if (currentEnemy != null)
                        {
                            ApplyDamageAndSynergies(currentEnemy, player);
                        }
                    }

                    if (explosionFXPrefab != null)
                    {
                        GameObject expFX = Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);
                        FX_Explosion fxScript = expFX.GetComponent<FX_Explosion>();
                        if (fxScript != null) fxScript.maxRadius = explosionRadius;

                        if (Camera.main != null)
                        {
                            CameraShake shaker = Camera.main.GetComponent<CameraShake>();
                            if (shaker != null) shaker.Shake(0.2f, 0.25f);
                        }
                    }
                }
                else
                {
                    EnemyAI enemyScript = collision.GetComponent<EnemyAI>();
                    if (enemyScript != null)
                    {
                        ApplyDamageAndSynergies(enemyScript, player);
                    }
                }
            }
            else
            {
                EnemyAI enemyScript = collision.GetComponent<EnemyAI>();
                if (enemyScript != null) enemyScript.TakeDamage(damage);
            }

            DeactivateProjectile();
        }
    }
    private void ApplyDamageAndSynergies(EnemyAI enemy, PlayerController player)
    {
        enemy.TakeDamage(damage);
        if (player.isDeadlineSynergyActive)
        {
            enemy.moveSpeed = Mathf.Max(enemy.moveSpeed * 0.5f, 0.5f);
        }
        if (player.isHomeOfficeSynergyActive)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - player.transform.position).normalized;
                enemyRb.AddForce(knockbackDirection * 15f, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}