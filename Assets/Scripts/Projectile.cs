using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    [Header("Synergy Explosion")]
    public float explosionRadius = 2.5f;
    public LayerMask enemyLayer;

    private GameObject fxPrefab;
    private GameObject explosionFXPrefab;

    public void Setup(Vector3 direction)
    {
        this.moveDirection = direction;
        fxPrefab = Resources.Load<GameObject>("FX_EmailDestroy");
        explosionFXPrefab = Resources.Load<GameObject>("FX_Explosion");
        Destroy(gameObject, lifetime);
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
            if (player != null && player.isOvertimeSynergyActive)
            {
                Collider2D[] surroundedEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

                foreach (Collider2D enemyCollider in surroundedEnemies)
                {
                    EnemyAI currentEnemy = enemyCollider.GetComponent<EnemyAI>();
                    if (currentEnemy != null)
                    {
                        currentEnemy.TakeDamage(damage);
                    }
                }
                if (explosionFXPrefab != null)
                {
                    GameObject expFX = Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);
                    FX_Explosion fxScript = expFX.GetComponent<FX_Explosion>();
                    if (fxScript != null)
                    {
                        fxScript.maxRadius = explosionRadius;
                    }
                    if (Camera.main != null)
                    {
                        CameraShake shaker = Camera.main.GetComponent<CameraShake>();
                        if (shaker != null)
                        {
                            shaker.Shake(0.2f, 0.25f);
                        }
                    }
                }

                Debug.Log($"[EXPLOSION] Sinerji patladı! {surroundedEnemies.Length} düşman hasar aldı.");
            }
            else
            {
                EnemyAI enemyScript = collision.GetComponent<EnemyAI>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}