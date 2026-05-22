using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyRole { Regular, Intern, HR_Manager, SeniorManager, Micromanager }

    [Header("Data Driven Setup")]
    public EnemyData enemyData;
    private float speedModifier = 1f;
    public float moveSpeed => enemyData != null ? enemyData.moveSpeed * speedModifier : 2.5f;
    public float health => enemyData != null ? enemyData.maxHealth : 20f;
    public float burnoutDamage => enemyData != null ? enemyData.burnoutDamage : 10f;

    public void ApplySlow(float slowAmount)
    {
        speedModifier = Mathf.Max(speedModifier * slowAmount, 0.2f);
    }

    private float currentHealth;
    private float nextFireTime = 0f;
    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
        }
    }
    private void OnEnable()
    {
        speedModifier = 1f;
        if (enemyData != null) currentHealth = enemyData.maxHealth;
    }
    void FixedUpdate()
    {
        if (playerTransform == null || rb == null || enemyData == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (enemyData.isRanged && distanceToPlayer <= enemyData.attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextFireTime)
            {
                FireReport();
                nextFireTime = Time.time + enemyData.fireRate;
            }
        }
        else
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }
    void FireReport()
    {
        if (playerTransform == null || ObjectPooler.Instance == null || enemyData == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 spawnOffset = direction * 0.8f;
        GameObject proj = ObjectPooler.Instance.SpawnFromPool(enemyData.poolTag, transform.position + spawnOffset, Quaternion.identity);

        if (proj != null)
        {
            EnemyProjectile projScript = proj.GetComponent<EnemyProjectile>();
            if (projScript == null) projScript = proj.AddComponent<EnemyProjectile>();

            projScript.Setup(direction, enemyData.burnoutDamage);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enemyData != null)
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.IncreaseBurnout(enemyData.burnoutDamage);
                gameObject.SetActive(false);
            }
        }
    }
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }
    void Die()
    {
        GameObject xpToSpawn = Resources.Load<GameObject>("Prefabs/XP_Signature");
        if (xpToSpawn != null) Instantiate(xpToSpawn, transform.position, Quaternion.identity);

        if (enemyData != null && enemyData.potentialDropItems != null && enemyData.potentialDropItems.Length > 0)
        {
            if (Random.value <= enemyData.dropChance)
            {
                string prefabToLoad = enemyData.potentialDropItems[Random.Range(0, enemyData.potentialDropItems.Length)];
                GameObject spawnItem = Resources.Load<GameObject>(prefabToLoad);
                if (spawnItem != null) Instantiate(spawnItem, transform.position, Quaternion.identity);
            }
        }
        gameObject.SetActive(false);
    }
}