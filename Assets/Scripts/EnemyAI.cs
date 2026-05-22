using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyRole { Regular, Intern, HR_Manager, SeniorManager, Micromanager }

    [Header("Enemy Stats")]
    public EnemyRole role = EnemyRole.Regular;
    public float moveSpeed = 2.5f;
    public float burnoutDamage = 10f;
    public float health = 20f;

    [Header("Ranged Attack Settings (Micromanager Only)")]
    public float attackRange = 5f;
    public float fireRate = 2f;
    private float nextFireTime = 0f;
    private GameObject enemyProjectilePrefab;

    [Header("Drops")]
    public GameObject xpPrefab;

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetupRoleStats();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        if (role == EnemyRole.Micromanager)
        {
            enemyProjectilePrefab = Resources.Load<GameObject>("EnemyProjectile_Report");
            if (enemyProjectilePrefab == null)
            {
                Debug.LogWarning("[ENEMY] 'EnemyProjectile_Report' prefab bulunamadı! Lütfen Resources klasörüne ekleyin.");
            }
        }
    }

    void SetupRoleStats()
    {
        switch (role)
        {
            case EnemyRole.Intern:
                moveSpeed = 4.5f;
                burnoutDamage = 5f;
                health = 5f;
                break;
            case EnemyRole.HR_Manager:
                moveSpeed = 1.2f;
                burnoutDamage = 25f;
                health = 60f;
                break;
            case EnemyRole.Micromanager:
                moveSpeed = 2.0f;
                burnoutDamage = 8f;
                health = 25f;
                break;
            default:
                moveSpeed = 2.5f;
                burnoutDamage = 10f;
                health = 20f;
                break;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null || rb == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (role == EnemyRole.Micromanager && distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextFireTime)
            {
                FireReport();
                nextFireTime = Time.time + fireRate;
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
        if (playerTransform == null || ObjectPooler.Instance == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 spawnOffset = direction * 0.8f;
        GameObject proj = ObjectPooler.Instance.SpawnFromPool("EnemyReport", transform.position + spawnOffset, Quaternion.identity);

        if (proj != null)
        {
            EnemyProjectile projScript = proj.GetComponent<EnemyProjectile>();
            if (projScript == null) projScript = proj.AddComponent<EnemyProjectile>();

            projScript.Setup(direction, burnoutDamage);
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

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0) Die();
    }

    void Die()
    {
        GameObject xpToSpawn = xpPrefab != null ? xpPrefab : Resources.Load<GameObject>("XP_Signature");
        if (xpToSpawn != null) Instantiate(xpToSpawn, transform.position, Quaternion.identity);

        float dropRoll = Random.value;
        switch (role)
        {
            case EnemyRole.SeniorManager:
                if (dropRoll <= 0.30f) DropItem(new string[] { "Item_Magnet", "Item_Espresso", "Item_LeaveRequest" });
                break;
            case EnemyRole.HR_Manager:
                if (dropRoll <= 0.40f) DropItem(new string[] { "Item_LeaveRequest" });
                break;
            case EnemyRole.Micromanager:
                if (dropRoll <= 0.20f) DropItem(new string[] { "Item_Espresso" });
                break;
        }
        Destroy(gameObject);
    }
    void DropItem(string[] potentialItems)
    {
        if (potentialItems.Length == 0) return;
        string prefabToLoad = potentialItems[Random.Range(0, potentialItems.Length)];
        GameObject spawnItem = Resources.Load<GameObject>(prefabToLoad);
        if (spawnItem != null) Instantiate(spawnItem, transform.position, Quaternion.identity);
    }
}