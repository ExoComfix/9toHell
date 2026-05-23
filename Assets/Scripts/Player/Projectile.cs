using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    [Header("Synergy Settings")]
    public float explosionRadius = 2.5f;
    public LayerMask enemyLayer;

    [Header("Pool Tags for Effects")]
    [SerializeField] private string hitFXPoolTag = "FX_EmailDestroy";
    [SerializeField] private string explosionFXPoolTag = "FX_Explosion";

    private PlayerController playerRef;

    public void Setup(Vector3 direction, PlayerController player = null)
    {
        this.moveDirection = direction;
        this.playerRef = player;

        CancelInvoke(nameof(DeactivateProjectile));
        Invoke(nameof(DeactivateProjectile), lifetime);
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HandleHitVisuals(collision.transform.position);

            if (playerRef != null)
            {
                if (playerRef.IsSynergyActive("Overtime"))
                {
                    ExecuteExplosionDamage();
                    HandleExplosionVisuals();
                }
                else
                {
                    EnemyAI enemyScript = collision.GetComponent<EnemyAI>();
                    if (enemyScript != null)
                    {
                        ApplyDamageAndSynergies(enemyScript);
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

    private void HandleHitVisuals(Vector3 position)
    {
        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.SpawnFromPool(hitFXPoolTag, position, Quaternion.identity);
        }
    }

    private void ExecuteExplosionDamage()
    {
        Collider2D[] surroundedEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D enemy in surroundedEnemies)
        {
            EnemyAI currentEnemy = enemy.GetComponent<EnemyAI>();
            if (currentEnemy != null)
            {
                ApplyDamageAndSynergies(currentEnemy);
            }
        }
    }

    private void HandleExplosionVisuals()
    {
        if (ObjectPooler.Instance != null)
        {
            GameObject expFX = ObjectPooler.Instance.SpawnFromPool(explosionFXPoolTag, transform.position, Quaternion.identity);
            if (expFX != null)
            {
                FX_Explosion fxScript = expFX.GetComponent<FX_Explosion>();
                if (fxScript != null) fxScript.maxRadius = explosionRadius;
            }
        }
        if (Camera.main != null)
        {
            CameraShake shaker = Camera.main.GetComponent<CameraShake>();
            if (shaker != null) shaker.Shake(0.2f, 0.25f);
        }
    }

    private void ApplyDamageAndSynergies(EnemyAI enemy)
    {
        if (playerRef == null) return;

        enemy.TakeDamage(damage);
        if (playerRef.IsSynergyActive("Deadline"))
        {
            enemy.ApplySlow(0.5f);
        }
        if (playerRef.IsSynergyActive("HomeOffice"))
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - playerRef.transform.position).normalized;
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